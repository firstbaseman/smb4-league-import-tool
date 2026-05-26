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

        public static string NormalizeRawGuidHex(string? rawHex)
        {
            if (string.IsNullOrWhiteSpace(rawHex))
                return string.Empty;

            string cleaned = rawHex.Trim();

            if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring(2);

            return cleaned
                .Replace("-", string.Empty)
                .ToUpperInvariant();
        }
        private static bool IsValidNormalizedRawGuidHex(string normalizedRawHex)
        {
            return normalizedRawHex.Length == 32 &&
                   normalizedRawHex.All(Uri.IsHexDigit);
        }

        public static bool IsValidRawGuidHex(string? rawHex)
        {
            string normalized = NormalizeRawGuidHex(rawHex);

            return IsValidNormalizedRawGuidHex(normalized);
        }

        public static bool IsDefaultLeagueGuidRaw(string rawHex)
        {
            string normalized = NormalizeRawGuidHex(rawHex);

            if (!IsValidNormalizedRawGuidHex(normalized))
                return false;

            return DefaultLeagueGuidsRaw.Contains(
                normalized,
                StringComparer.OrdinalIgnoreCase);
        }

        public static string FormatGuidWithDashes(string rawHex)
        {
            string normalized = NormalizeRawGuidHex(rawHex);

            if (!IsValidNormalizedRawGuidHex(normalized))
                return normalized;

            return string.Format(
                "{0}-{1}-{2}-{3}-{4}",
                normalized.Substring(0, 8),
                normalized.Substring(8, 4),
                normalized.Substring(12, 4),
                normalized.Substring(16, 4),
                normalized.Substring(20));
        }
        public static string ToDisplayGuid(string? rawHex)
        {
            string normalized = NormalizeRawGuidHex(rawHex);

            return string.IsNullOrEmpty(normalized)
                ? "N/A"
                : FormatGuidWithDashes(normalized);
        }
        public static byte[] HexToBytes(string hex)
        {
            string normalized = NormalizeRawGuidHex(hex);

            if (string.IsNullOrWhiteSpace(normalized))
                return Array.Empty<byte>();

            if (normalized.Length % 2 != 0)
                throw new ArgumentException("Hex string has an invalid length.", nameof(hex));

            if (!normalized.All(Uri.IsHexDigit))
                throw new ArgumentException("Hex string contains non-hex characters.", nameof(hex));

            return Convert.FromHexString(normalized);
        }
    }
}