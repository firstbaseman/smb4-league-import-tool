namespace SMB4LeagueImportTool.Models
{
    public static class LeagueKindExtensions
    {
        public static string ToDisplayText(this LeagueKind kind)
        {
            return kind switch
            {
                LeagueKind.Default => "Default",
                LeagueKind.Custom => "Custom",
                LeagueKind.Franchise => "Franchise",
                LeagueKind.Unknown => "Unknown",
                _ => "Unknown"
            };
        }
    }
}
