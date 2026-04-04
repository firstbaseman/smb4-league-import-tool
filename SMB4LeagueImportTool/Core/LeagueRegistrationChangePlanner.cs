using SMB4LeagueImportTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMB4LeagueImportTool.Core
{
    /// <summary>
    /// Builds a save plan from the current league grid state.
    /// Does not touch WinForms, SQLite, or the filesystem.
    /// </summary>
    public static class LeagueRegistrationChangePlanner
    {
        public static LeagueRegistrationChangePlan BuildPlan(
            IEnumerable<LeagueRowInfo> rows,
            ISet<string> initialRegisteredGuids)
        {
            ArgumentNullException.ThrowIfNull(rows);
            ArgumentNullException.ThrowIfNull(initialRegisteredGuids);

            var plan = new LeagueRegistrationChangePlan();

            foreach (var info in rows)
            {
                if (!info.IsRegistered)
                    continue;

                if (string.IsNullOrWhiteSpace(info.RawGuidHex))
                    continue;

                string rawGuid = info.RawGuidHex.Trim().ToUpperInvariant();

                plan.NewRegisteredGuids.Add(rawGuid);

                bool isMissingSave =
                    string.IsNullOrWhiteSpace(info.SaveFileName) &&
                    !LeagueGuidHelper.IsDefaultLeagueGuidRaw(rawGuid);

                if (isMissingSave)
                    plan.MissingCheckedSaves.Add(info);
            }

            var newRegisteredSet = new HashSet<string>(
                plan.NewRegisteredGuids,
                StringComparer.OrdinalIgnoreCase);

            plan.HasChanges =
                !newRegisteredSet.SetEquals(initialRegisteredGuids);

            return plan;
        }
    }
}