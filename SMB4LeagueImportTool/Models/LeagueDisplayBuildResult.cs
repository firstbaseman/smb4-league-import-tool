using System;
using System.Collections.Generic;

namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueDisplayBuildResult
    {
        public List<LeagueRowInfo> RowsInDisplayOrder { get; } = new();

        public int DefaultCount { get; set; }

        public int CustomCount { get; set; }

        public int FranchiseCount { get; set; }

        public int InitialRegisteredCount { get; set; }

        public HashSet<string> InitialRegisteredGuids { get; } =
            new(StringComparer.OrdinalIgnoreCase);
    }
}