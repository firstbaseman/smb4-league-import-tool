namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueRowViewModel(
        LeagueRowInfo info,
        bool isRegistered)
    {
        public LeagueRowInfo Info { get; } = info;

        public bool IsRegistered { get; } = isRegistered;

        public bool IsDefaultLeague => Info.IsDefaultLeague;

        public bool IsCustomLeague => Info.IsCustomLeague;

        public bool IsFranchise => Info.IsFranchise;

        public bool HasSaveFile => Info.HasSaveFile;

        public bool HasValidGuid => Info.HasValidGuid;

        public bool CanBeRegistered => Info.CanBeRegistered;

        public bool IsMissingNonDefaultSave => Info.IsMissingNonDefaultSave;
    }
}