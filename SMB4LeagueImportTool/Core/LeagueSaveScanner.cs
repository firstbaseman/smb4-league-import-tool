using Microsoft.Data.Sqlite;
using SMB4LeagueImportTool.Models;

namespace SMB4LeagueImportTool.Core
{
    /// <summary>
    /// Scans league-*.sav files, reads their metadata, classifies them,
    /// and optionally repairs filename/internal GUID mismatches.
    /// </summary>
    public static class LeagueSaveScanner
    {
        public static LeagueSaveScanResult ScanLeagueSaveFiles(
            string savesFolderPath,
            IReadOnlyList<string> leagueSaveFiles,
            Func<LeagueFilenameMismatchInfo, bool> shouldRepairFilenameMismatch)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                throw new ArgumentException("Saves folder path cannot be empty.", nameof(savesFolderPath));

            ArgumentNullException.ThrowIfNull(leagueSaveFiles);
            ArgumentNullException.ThrowIfNull(shouldRepairFilenameMismatch);

            var result = new LeagueSaveScanResult();

            using var savManager = new SavManager();

            foreach (var originalLeagueSavPath in leagueSaveFiles)
            {
                string leagueSavPath = originalLeagueSavPath;
                string fileName = Path.GetFileName(leagueSavPath);
                string tempSqlitePath;

                try
                {
                    tempSqlitePath = savManager.DecompressSavToTemp(leagueSavPath);
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"Failed to decompress league save: {fileName}", ex);

                    var brokenInfo = new LeagueRowInfo
                    {
                        RawGuidHex = string.Empty,
                        DisplayGuid = "N/A",
                        Name = Path.GetFileNameWithoutExtension(fileName) + " (failed to open)",
                        Kind = LeagueKind.Unknown,
                        SaveFileName = fileName
                    };

                    result.LeagueInfos[fileName] = brokenInfo;
                    continue;
                }

                string rawGuid = string.Empty;
                string displayName = Path.GetFileNameWithoutExtension(fileName);
                bool isFranchise = false;

                using (var conn = SqliteConnectionFactory.CreateReadOnly(tempSqlitePath))
                {
                    conn.Open();

                    using (var cmd = new SqliteCommand(Smb4SqlQueries.ReadLeagueGuidAndName, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                rawGuid = LeagueGuidHelper.NormalizeRawGuidHex(reader.GetString(0));

                            if (!reader.IsDBNull(1))
                                displayName = reader.GetString(1);
                        }
                    }

                    try
                    {
                        using var franchiseCmd =
                            new SqliteCommand(Smb4SqlQueries.DetectFranchiseSave, conn);

                        using var franchiseReader = franchiseCmd.ExecuteReader();
                        isFranchise = franchiseReader.Read();
                    }
                    catch (SqliteException)
                    {
                        // Table may not exist in pure league saves; that's fine.
                        isFranchise = false;
                    }
                }

                TryRepairFilenameMismatch(
                    result,
                    originalLeagueSavPath,
                    ref leagueSavPath,
                    ref fileName,
                    rawGuid,
                    displayName,
                    shouldRepairFilenameMismatch);

                LeagueKind kind;

                if (!string.IsNullOrEmpty(rawGuid) &&
                    LeagueGuidHelper.IsDefaultLeagueGuidRaw(rawGuid))
                {
                    kind = LeagueKind.Default;
                }
                else
                {
                    kind = isFranchise ? LeagueKind.Franchise : LeagueKind.Custom;
                }

                var info = new LeagueRowInfo
                {
                    RawGuidHex = rawGuid,
                    DisplayGuid = LeagueGuidHelper.ToDisplayGuid(rawGuid),
                    Name = displayName,
                    Kind = kind,
                    SaveFileName = fileName
                };

                string key = string.IsNullOrEmpty(rawGuid) ? fileName : rawGuid;
                result.LeagueInfos[key] = info;
            }

            AppLogger.Info($"Scanned {leagueSaveFiles.Count} league save file(s).");

            return result;
        }

        private static void TryRepairFilenameMismatch(
            LeagueSaveScanResult result,
            string originalLeagueSavPath,
            ref string leagueSavPath,
            ref string fileName,
            string rawGuid,
            string displayName,
            Func<LeagueFilenameMismatchInfo, bool> shouldRepairFilenameMismatch)
        {
            try
            {
                if (string.IsNullOrEmpty(rawGuid))
                    return;

                if (!LeagueGuidHelper.IsValidRawGuidHex(rawGuid))
                    return;

                string newFileName = LeagueSaveFileNameHelper.GetExpectedFileName(rawGuid);

                bool hasFilenameMismatch =
                    !string.Equals(fileName, newFileName, StringComparison.OrdinalIgnoreCase);

                if (!hasFilenameMismatch)
                    return;

                string newPath = Path.Combine(Path.GetDirectoryName(leagueSavPath)!, newFileName);

                var mismatch = new LeagueFilenameMismatchInfo
                {
                    OldName = Path.GetFileName(originalLeagueSavPath),
                    CorrectName = newFileName,
                    LeagueName = displayName
                };

                bool shouldRepair = shouldRepairFilenameMismatch(mismatch);

                if (shouldRepair)
                {
                    if (File.Exists(newPath))
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
                        string backupPath = newPath + "." + timestamp + ".bak";

                        File.Move(newPath, backupPath);
                    }

                    File.Move(leagueSavPath, newPath);

                    leagueSavPath = newPath;
                    fileName = Path.GetFileName(newPath);

                    result.RenamedSaves.Add((
                        Path.GetFileName(originalLeagueSavPath),
                        fileName,
                        displayName));

                    AppLogger.Info(
                        $"Renamed league save '{Path.GetFileName(originalLeagueSavPath)}' to '{fileName}' for '{displayName}'.");
                }
                else
                {
                    result.SkippedRenames.Add((
                        Path.GetFileName(originalLeagueSavPath),
                        newFileName,
                        displayName));

                    AppLogger.Warning(
                        $"Skipped filename repair for '{Path.GetFileName(originalLeagueSavPath)}'. Correct filename would be '{newFileName}' for '{displayName}'.");
                }
            }
            catch (Exception ex)
            {
                result.FailedRenames.Add((Path.GetFileName(originalLeagueSavPath), ex.Message));

                AppLogger.Error(
                    $"Failed to repair filename for '{Path.GetFileName(originalLeagueSavPath)}'.",
                    ex);
            }
        }
    }
}