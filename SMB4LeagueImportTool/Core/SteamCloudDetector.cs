namespace SMB4LeagueImportTool.Core
{
    public static class SteamCloudDetector
    {
        public static bool IsDetected(string? savesFolderPath)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
                return false;

            if (!Directory.Exists(savesFolderPath))
                return false;

            string autoCloudPath = Path.Combine(
                savesFolderPath,
                Smb4SaveConstants.SteamAutoCloudFileName);

            return File.Exists(autoCloudPath);
        }
    }
}