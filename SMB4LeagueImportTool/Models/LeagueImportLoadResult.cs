namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueImportLoadResult
    {
        public bool HasLeagueSaveFiles { get; init; }

        public int LeagueSaveFileCount { get; init; }

        public string StatusText { get; init; } = string.Empty;

        public LeagueDisplayBuildResult? DisplayBuild { get; init; }

        public List<(string OldName, string NewName, string LeagueName)> RenamedSaves { get; } = new();

        public List<(string OldName, string CorrectName, string LeagueName)> SkippedRenames { get; } = new();

        public List<(string FileName, string Reason)> FailedRenames { get; } = new();
    }
}