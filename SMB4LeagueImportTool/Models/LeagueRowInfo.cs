namespace SMB4LeagueImportTool.Models
{
    // Core data for a single league/franchise row.
    // Backed by data from master.sav and each league-*.sav file.
    public sealed class LeagueRowInfo
    {
        public string RawGuidHex { get; init; } = string.Empty;
        public string DisplayGuid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public LeagueKind Kind { get; init; } = LeagueKind.Unknown;
        public string SaveFileName { get; init; } = string.Empty;

        public bool HasValidGuid => !string.IsNullOrWhiteSpace(RawGuidHex);

        public bool HasSaveFile => !string.IsNullOrWhiteSpace(SaveFileName);

        public bool CanBeRegistered => HasValidGuid && HasSaveFile;

        public bool IsDefaultLeague => Kind == LeagueKind.Default;

        public bool IsCustomLeague => Kind == LeagueKind.Custom;

        public bool IsFranchise => Kind == LeagueKind.Franchise;

        public bool IsMissingNonDefaultSave =>
            HasValidGuid &&
            !HasSaveFile &&
            !IsDefaultLeague;
    }
}