using System;

namespace SMB4LeagueImportTool.Core
{
    public static class LeagueGuidHelper
    {
        // Raw hex, no dashes, as stored in t_league_savedatas.GUID.
        private static readonly string[] DefaultLeagueGuidsRaw =
        {
            "99F30082775B4547ADD88C7D2C94FCE5",
            "1EE40D82453A474082E50827731C22E0",
            "7CBC32B9BD7F48D7AE0144C6595CD5A6"
        };

        public static bool IsDefaultLeagueGuidRaw(string rawHex)
        {
            if (string.IsNullOrEmpty(rawHex))
                return false;

            string upper = rawHex.ToUpperInvariant();

            foreach (var defaultGuid in DefaultLeagueGuidsRaw)
            {
                if (upper == defaultGuid)
                    return true;
            }

            return false;
        }

        public static string FormatGuidWithDashes(string rawHex)
        {
            if (string.IsNullOrWhiteSpace(rawHex))
                return string.Empty;

            string upper = rawHex.ToUpperInvariant();

            if (upper.Length != 32)
                return upper;

            return string.Format(
                "{0}-{1}-{2}-{3}-{4}",
                upper.Substring(0, 8),
                upper.Substring(8, 4),
                upper.Substring(12, 4),
                upper.Substring(16, 4),
                upper.Substring(20));
        }

        public static byte[] HexToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Array.Empty<byte>();

            string cleaned = hex.Trim();

            if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring(2);

            if (cleaned.Length % 2 != 0)
                throw new ArgumentException("Hex string has an invalid length.", nameof(hex));

            return Convert.FromHexString(cleaned);
        }
    }
}