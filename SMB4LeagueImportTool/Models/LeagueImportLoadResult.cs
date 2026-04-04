using System;
using System.Collections.Generic;
using System.Text;

namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueImportLoadResult
    {
        public bool HasLeagueSaveFiles { get; set; }

        public int LeagueSaveFileCount { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public LeagueDisplayBuildResult? DisplayBuild { get; set; }

        public List<(string OldName, string NewName, string LeagueName)> RenamedSaves { get; } = new();

        public List<(string OldName, string CorrectName, string LeagueName)> SkippedRenames { get; } = new();

        public List<(string FileName, string Reason)> FailedRenames { get; } = new();
    }
}