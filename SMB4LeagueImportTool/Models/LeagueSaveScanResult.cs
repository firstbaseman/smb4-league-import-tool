using System;
using System.Collections.Generic;
using System.Text;

namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueSaveScanResult
    {
        public Dictionary<string, LeagueRowInfo> LeagueInfos { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public List<(string OldName, string NewName, string LeagueName)> RenamedSaves { get; } = new();

        public List<(string OldName, string CorrectName, string LeagueName)> SkippedRenames { get; } = new();

        public List<(string FileName, string Reason)> FailedRenames { get; } = new();
    }
}