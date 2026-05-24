using Microsoft.Win32;
using System;
using System.Linq;

namespace SMB4LeagueImportTool.Core
{
    public static class RuntimeEnvironmentInfo
    {
        public static bool IsProbablyWineOrProton()
        {
            return HasWineRegistryKey() ||
                   HasWineEnvironmentVariables() ||
                   HasProtonEnvironmentVariables();
        }

        public static bool IsExpectedSmb4SteamContext()
        {
            string steamAppId = Environment.GetEnvironmentVariable("SteamAppId") ?? string.Empty;
            string steamGameId = Environment.GetEnvironmentVariable("SteamGameId") ?? string.Empty;
            string steamCompatDataPath = Environment.GetEnvironmentVariable("STEAM_COMPAT_DATA_PATH") ?? string.Empty;

            if (string.Equals(steamAppId, Smb4SaveConstants.SteamAppId, StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(steamGameId, Smb4SaveConstants.SteamAppId, StringComparison.OrdinalIgnoreCase))
                return true;

            return PathContainsSteamAppIdSegment(
                steamCompatDataPath,
                Smb4SaveConstants.SteamAppId);
        }

        public static string GetRuntimeLabel()
        {
            return IsProbablyWineOrProton()
                ? "Wine/Proton compatibility environment detected"
                : "Native Windows environment detected";
        }

        public static string GetEnvironmentDetails()
        {
            string winePrefix = Environment.GetEnvironmentVariable("WINEPREFIX") ?? string.Empty;
            string steamCompatDataPath = Environment.GetEnvironmentVariable("STEAM_COMPAT_DATA_PATH") ?? string.Empty;
            string steamAppId = Environment.GetEnvironmentVariable("SteamAppId") ?? string.Empty;
            string steamGameId = Environment.GetEnvironmentVariable("SteamGameId") ?? string.Empty;
            string xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME") ?? string.Empty;
            string home = Environment.GetEnvironmentVariable("HOME") ?? string.Empty;

            return
                $"Wine registry key detected: {HasWineRegistryKey()}{Environment.NewLine}" +
                $"WINEPREFIX: {ValueOrNone(winePrefix)}{Environment.NewLine}" +
                $"STEAM_COMPAT_DATA_PATH: {ValueOrNone(steamCompatDataPath)}{Environment.NewLine}" +
                $"SteamAppId: {ValueOrNone(steamAppId)}{Environment.NewLine}" +
                $"SteamGameId: {ValueOrNone(steamGameId)}{Environment.NewLine}" +
                $"XDG_DATA_HOME: {ValueOrNone(xdgDataHome)}{Environment.NewLine}" +
                $"HOME: {ValueOrNone(home)}{Environment.NewLine}" +
                $"Expected SMB4 Steam App ID: {Smb4SaveConstants.SteamAppId}{Environment.NewLine}" +
                $"Running in expected SMB4 Steam context: {IsExpectedSmb4SteamContext()}";
        }

        private static bool HasWineRegistryKey()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Wine");
                return key is not null;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasWineEnvironmentVariables()
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WINEPREFIX"));
        }

        private static bool HasProtonEnvironmentVariables()
        {
            return
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("STEAM_COMPAT_DATA_PATH")) ||
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SteamAppId")) ||
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SteamGameId"));
        }

        private static bool PathContainsSteamAppIdSegment(
            string path,
            string steamAppId)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(steamAppId))
                return false;

            string normalized = path.Replace('\\', '/');

            string[] segments = normalized.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

            return segments.Any(segment =>
                string.Equals(segment, steamAppId, StringComparison.OrdinalIgnoreCase));
        }

        private static string ValueOrNone(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(none)" : value;
        }
    }
}