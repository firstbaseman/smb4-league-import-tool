using Microsoft.Data.Sqlite;

namespace SMB4LeagueImportTool.Core
{
    // Handles cleanup of temporary files and legacy temp folders.
    // Logging and cleanup are best-effort; cleanup should never crash the app.
    public static class TempCleanupService
    {
        public static void CleanupForSavesFolder(string? savesFolderPath)
        {
            try
            {
                AppLogger.Info("Cleanup started.");

                // Make sure no pooled SQLite connections are still holding onto temp files.
                SqliteConnection.ClearAllPools();

                CleanupLegacyTempFolder(savesFolderPath);

                AppLogger.Info("Cleanup finished.");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cleanup failed.", ex);
                // Cleanup is best-effort only.
            }
        }

        private static void CleanupLegacyTempFolder(string? savesFolderPath)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                return;

            string oldTempFolder = Path.Combine(
                savesFolderPath,
                Smb4SaveConstants.LegacyTempFolderName);

            if (!Directory.Exists(oldTempFolder))
                return;

            Directory.Delete(oldTempFolder, recursive: true);

            AppLogger.Info($"Deleted old temp folder: {oldTempFolder}");
        }
    }
}