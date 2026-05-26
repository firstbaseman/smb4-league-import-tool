using System;
using System.Collections.Generic;

namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueDisplayBuildResult
    {
        public List<LeagueRowViewModel> RowsInDisplayOrder { get; } = new();

        public int DefaultCount { get; init; }

        public int CustomCount { get; init; }

        public int FranchiseCount { get; init; }

        public int InitialRegisteredCount { get; init; }

        public HashSet<string> InitialRegisteredGuids { get; } =
            new(StringComparer.OrdinalIgnoreCase);
    }
}