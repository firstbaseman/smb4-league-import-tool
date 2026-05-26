using SMB4LeagueImportTool.Models;

namespace SMB4LeagueImportTool.Core
{
    // Handles filesystem export logic for league/franchise .sav files.
    public static class LeagueSaveExporter
    {
        public static string GetSourcePath(
            string savesFolderPath,
            LeagueRowInfo leagueInfo)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                throw new ArgumentException("Saves folder path cannot be empty.", nameof(savesFolderPath));

            ArgumentNullException.ThrowIfNull(leagueInfo);

            if (!leagueInfo.HasSaveFile)
                throw new InvalidOperationException(
                    "This entry does not have a corresponding league-*.sav file to export.");

            string sourcePath = Path.Combine(savesFolderPath, leagueInfo.SaveFileName);

            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("The underlying save file could not be found.", sourcePath);

            return sourcePath;
        }

        public static void Export(
            string savesFolderPath,
            LeagueRowInfo leagueInfo,
            string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("Destination path cannot be empty.", nameof(destinationPath));

            string sourcePath = GetSourcePath(savesFolderPath, leagueInfo);

            File.Copy(sourcePath, destinationPath, overwrite: true);

            AppLogger.Info(
                $"Exported '{leagueInfo.Name}' from '{sourcePath}' to '{destinationPath}'.");
        }
    }
}