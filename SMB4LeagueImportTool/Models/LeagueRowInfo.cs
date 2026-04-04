using System;
using System.Collections.Generic;
using System.Text;

namespace SMB4LeagueImportTool.Models
{
    // Flattened view model for a single league/franchise row in the grid.
    // Backed by data from master.sav and each league-*.sav file.
    public sealed class LeagueRowInfo
    {
        public string RawGuidHex { get; set; } = string.Empty;
        public string DisplayGuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRegistered { get; set; }
        public string SaveFileName { get; set; } = string.Empty;
    }
}