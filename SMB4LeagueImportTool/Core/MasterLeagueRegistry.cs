using Microsoft.Data.Sqlite;

namespace SMB4LeagueImportTool.Core
{
    // Reads and writes the league/franchise registration list stored in master.sav.
    public static class MasterLeagueRegistry
    {
        public static List<string> ReadRegisteredGuids(string savesFolderPath)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                throw new ArgumentException("Saves folder path cannot be empty.", nameof(savesFolderPath));

            string masterSavPath = Path.Combine(
                savesFolderPath,
                Smb4SaveConstants.MasterSaveFileName);

            if (!File.Exists(masterSavPath))
                throw new FileNotFoundException("master.sav was not found.", masterSavPath);

            var registeredGuids = new List<string>();

            using var savManager = new SavManager();
            string masterSqlitePath = savManager.DecompressSavToTemp(masterSavPath);

            using var conn = SqliteConnectionFactory.CreateReadOnly(masterSqlitePath);

            conn.Open();

            using var cmd = new SqliteCommand(
                Smb4SqlQueries.ReadRegisteredLeagueGuids,
                conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string rawHex = reader.IsDBNull(0)
                    ? string.Empty
                    : LeagueGuidHelper.NormalizeRawGuidHex(reader.GetString(0));

                int isMissing = reader.IsDBNull(1)
                    ? 0
                    : reader.GetInt32(1);

                if (!LeagueGuidHelper.IsValidRawGuidHex(rawHex))
                    continue;

                // SMB4 treats missing entries as invalid; do not treat them as registered.
                if (isMissing != 0)
                    continue;

                registeredGuids.Add(rawHex);
            }

            AppLogger.Info($"Read {registeredGuids.Count} registered GUID(s) from master.sav.");

            return registeredGuids;
        }

        public static void RewriteRegisteredGuids(
            string savesFolderPath,
            IEnumerable<string> rawGuidHexValues)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                throw new ArgumentException("Saves folder path cannot be empty.", nameof(savesFolderPath));

            ArgumentNullException.ThrowIfNull(rawGuidHexValues);

            string masterSavPath = Path.Combine(
                savesFolderPath,
                Smb4SaveConstants.MasterSaveFileName);

            if (!File.Exists(masterSavPath))
                throw new FileNotFoundException("master.sav was not found.", masterSavPath);

            var normalizedGuids = rawGuidHexValues
                .Select(LeagueGuidHelper.NormalizeRawGuidHex)
                .Where(LeagueGuidHelper.IsValidRawGuidHex)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            using var savManager = new SavManager();
            string tempSqlitePath = savManager.DecompressSavToTemp(masterSavPath);

            using (var conn = SqliteConnectionFactory.CreateReadWrite(tempSqlitePath))
            {
                conn.Open();

                using var tx = conn.BeginTransaction();

                using (var deleteCmd = conn.CreateCommand())
                {
                    deleteCmd.CommandText = Smb4SqlQueries.DeleteRegisteredLeagueGuids;
                    deleteCmd.Transaction = tx;
                    deleteCmd.ExecuteNonQuery();
                }

                using (var insertCmd = conn.CreateCommand())
                {
                    insertCmd.CommandText = Smb4SqlQueries.InsertRegisteredLeagueGuid;

                    insertCmd.Transaction = tx;

                    var guidParam = insertCmd.CreateParameter();
                    guidParam.ParameterName = "@guid";
                    guidParam.SqliteType = SqliteType.Blob;
                    insertCmd.Parameters.Add(guidParam);

                    foreach (var rawHex in normalizedGuids)
                    {
                        guidParam.Value = LeagueGuidHelper.HexToBytes(rawHex);
                        insertCmd.ExecuteNonQuery();
                    }
                }

                tx.Commit();
            }

            // Ensure no pooled connections are still holding the temp SQLite file.
            SqliteConnection.ClearAllPools();

            savManager.RepackTempSqliteToSav(tempSqlitePath, masterSavPath);

            AppLogger.Info(
                $"Rewrote master.sav registration list with {normalizedGuids.Count} GUID(s).");
        }
    }
}