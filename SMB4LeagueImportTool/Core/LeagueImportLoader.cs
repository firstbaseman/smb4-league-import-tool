using SMB4LeagueImportTool.Models;

namespace SMB4LeagueImportTool.Core
{
    // Coordinates the non-UI league import loading pipeline:
    // finding league saves, reading master.sav registrations,
    // scanning league metadata, and building display-ready rows.
    public static class LeagueImportLoader
    {
        public static LeagueImportLoadResult Load(
            string savesFolderPath,
            Func<LeagueFilenameMismatchInfo, bool> shouldRepairFilenameMismatch)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                throw new ArgumentException("Saves folder path cannot be empty.", nameof(savesFolderPath));

            ArgumentNullException.ThrowIfNull(shouldRepairFilenameMismatch);

            string[] leagueSaveFiles = Directory.GetFiles(
                savesFolderPath,
                Smb4SaveConstants.LeagueSaveSearchPattern,
                SearchOption.TopDirectoryOnly);

            AppLogger.Info($"Detected {leagueSaveFiles.Length} league-*.sav file(s).");

            if (leagueSaveFiles.Length == 0)
            {
                return new LeagueImportLoadResult
                {
                    LeagueSaveFileCount = leagueSaveFiles.Length,
                    HasLeagueSaveFiles = false,
                    StatusText = "master.sav found, but no league-*.sav files were detected."
                };
            }

            var registeredGuids = MasterLeagueRegistry.ReadRegisteredGuids(savesFolderPath);

            var scanResult = LeagueSaveScanner.ScanLeagueSaveFiles(
                savesFolderPath,
                leagueSaveFiles,
                shouldRepairFilenameMismatch);

            var displayBuild = LeagueDisplayBuilder.Build(
                scanResult.LeagueInfos,
                registeredGuids);

            var result = new LeagueImportLoadResult
            {
                LeagueSaveFileCount = leagueSaveFiles.Length,
                HasLeagueSaveFiles = true,
                DisplayBuild = displayBuild
            };

            result.RenamedSaves.AddRange(scanResult.RenamedSaves);
            result.SkippedRenames.AddRange(scanResult.SkippedRenames);
            result.FailedRenames.AddRange(scanResult.FailedRenames);

            return result;
        }
    }
}