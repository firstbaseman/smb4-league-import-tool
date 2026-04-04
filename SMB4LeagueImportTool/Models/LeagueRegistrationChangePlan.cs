using System;
using System.Collections.Generic;
using System.Text;

namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueRegistrationChangePlan
    {
        public List<string> NewRegisteredGuids { get; } = new();

        public List<LeagueRowInfo> MissingCheckedSaves { get; } = new();

        public int NewRegisteredCount => NewRegisteredGuids.Count;

        public bool HasChanges { get; set; }
    }
}