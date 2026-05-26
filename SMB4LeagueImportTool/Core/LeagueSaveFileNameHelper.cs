using System;
using System.IO;

namespace SMB4LeagueImportTool.Core
{
    public static class LeagueSaveFileNameHelper
    {
        public static bool IsLeagueSaveFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            string nameOnly = Path.GetFileName(fileName) ?? string.Empty;

            return nameOnly.StartsWith(Smb4SaveConstants.LeagueFilePrefix, StringComparison.OrdinalIgnoreCase) &&
                   nameOnly.EndsWith(Smb4SaveConstants.SaveFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryGetRawGuidFromFileName(
            string? fileName,
            out string rawGuidHex)
        {
            rawGuidHex = string.Empty;

            if (!IsLeagueSaveFileName(fileName))
                return false;

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName) ?? string.Empty;

            if (!nameWithoutExtension.StartsWith(Smb4SaveConstants.LeagueFilePrefix, StringComparison.OrdinalIgnoreCase))
                return false;

            string guidPart = nameWithoutExtension.Substring(Smb4SaveConstants.LeagueFilePrefix.Length);

            if (!LeagueGuidHelper.IsValidRawGuidHex(guidPart))
                return false;

            rawGuidHex = LeagueGuidHelper.NormalizeRawGuidHex(guidPart);
            return true;
        }

        public static string GetExpectedFileName(string rawGuidHex)
        {
            string normalized = LeagueGuidHelper.NormalizeRawGuidHex(rawGuidHex);

            if (!LeagueGuidHelper.IsValidRawGuidHex(normalized))
                throw new ArgumentException("Invalid league GUID.", nameof(rawGuidHex));

            string dashedGuid = LeagueGuidHelper.FormatGuidWithDashes(normalized);

            return $"{Smb4SaveConstants.LeagueFilePrefix}{dashedGuid}{Smb4SaveConstants.SaveFileExtension}";
        }
    }
}