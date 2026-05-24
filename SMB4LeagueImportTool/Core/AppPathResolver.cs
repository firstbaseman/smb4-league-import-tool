using System;
using System.IO;
using System.Linq;

namespace SMB4LeagueImportTool.Core
{
    public static class AppPathResolver
    {
        public static string GetLogFolderPath()
        {
            if (RuntimeEnvironmentInfo.IsProbablyWineOrProton())
            {
                string? linuxFriendlyLogPath = TryGetLinuxFriendlyLogPath();

                if (!string.IsNullOrWhiteSpace(linuxFriendlyLogPath))
                    return linuxFriendlyLogPath;
            }

            return GetWindowsLocalAppDataLogPath();
        }

        public static string GetLogFolderDisplayPath()
        {
            string logFolderPath = GetLogFolderPath();

            if (TryConvertWineZPathToUnixPath(logFolderPath, out string unixPath))
                return unixPath;

            return logFolderPath;
        }

        private static string GetWindowsLocalAppDataLogPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SMB4LeagueImportTool",
                "Logs");
        }

        private static string? TryGetLinuxFriendlyLogPath()
        {
            string? xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");

            if (LooksLikeUnixAbsolutePath(xdgDataHome))
            {
                return ConvertUnixPathToWinePath(
                    CombineUnixPath(xdgDataHome!, "SMB4LeagueImportTool", "Logs"));
            }

            string? home = Environment.GetEnvironmentVariable("HOME");

            if (LooksLikeUnixAbsolutePath(home))
            {
                return ConvertUnixPathToWinePath(
                    CombineUnixPath(home!, ".local", "share", "SMB4LeagueImportTool", "Logs"));
            }

            string? steamCompatDataPath = Environment.GetEnvironmentVariable("STEAM_COMPAT_DATA_PATH");
            string? derivedHome = TryDeriveHomeFromUnixPath(steamCompatDataPath);

            if (LooksLikeUnixAbsolutePath(derivedHome))
            {
                return ConvertUnixPathToWinePath(
                    CombineUnixPath(derivedHome!, ".local", "share", "SMB4LeagueImportTool", "Logs"));
            }

            return null;
        }

        private static string? TryDeriveHomeFromUnixPath(string? unixPath)
        {
            if (!LooksLikeUnixAbsolutePath(unixPath))
                return null;

            string normalized = unixPath!.Replace('\\', '/');

            // Common cases:
            // /home/deck/.local/share/Steam/steamapps/compatdata/...
            // /home/ari/.steam/steam/steamapps/compatdata/...
            if (!normalized.StartsWith("/home/", StringComparison.Ordinal))
                return null;

            string[] parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                return null;

            // parts[0] = home
            // parts[1] = username
            return "/home/" + parts[1];
        }

        private static string ConvertUnixPathToWinePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            // Already Windows-style.
            if (path.Length >= 2 && path[1] == ':')
                return path;

            string normalized = path.Replace('\\', '/');

            // Wine commonly maps Linux filesystem root "/" to Z:\.
            // This can be changed by users, so this is best-effort.
            if (normalized.StartsWith("/", StringComparison.Ordinal))
                return "Z:" + normalized.Replace('/', '\\');

            return normalized.Replace('/', '\\');
        }

        private static bool TryConvertWineZPathToUnixPath(
            string path,
            out string unixPath)
        {
            unixPath = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (!path.StartsWith(@"Z:\", StringComparison.OrdinalIgnoreCase))
                return false;

            unixPath = "/" + path.Substring(3).Replace('\\', '/');
            return true;
        }

        private static bool LooksLikeUnixAbsolutePath(string? path)
        {
            return !string.IsNullOrWhiteSpace(path) &&
                   path.StartsWith("/", StringComparison.Ordinal);
        }

        private static string CombineUnixPath(params string[] parts)
        {
            return string.Join(
                "/",
                parts
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim('/')));
        }
    }
}