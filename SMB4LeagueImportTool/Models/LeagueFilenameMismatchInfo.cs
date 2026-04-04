using System;
using System.Collections.Generic;
using System.Text;

namespace SMB4LeagueImportTool.Models
{
    public sealed class LeagueFilenameMismatchInfo
    {
        public string OldName { get; set; } = string.Empty;
        public string CorrectName { get; set; } = string.Empty;
        public string LeagueName { get; set; } = string.Empty;
    }
}
