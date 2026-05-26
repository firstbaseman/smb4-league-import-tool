using SMB4LeagueImportTool.Models;

namespace SMB4LeagueImportTool.Core
{
    /// <summary>
    /// Builds a save plan from the current league grid state.
    /// Does not touch WinForms, SQLite, or the filesystem.
    /// </summary>
    public static class LeagueRegistrationChangePlanner
    {
        public static LeagueRegistrationChangePlan BuildPlan(
            IEnumerable<(LeagueRowViewModel RowModel, bool IsRegistered)> currentRows,
            IReadOnlySet<string> initialRegisteredGuids)
        {
            ArgumentNullException.ThrowIfNull(currentRows);
            ArgumentNullException.ThrowIfNull(initialRegisteredGuids);

            var plan = new LeagueRegistrationChangePlan();

            foreach (var row in currentRows)
            {
                if (!row.IsRegistered)
                    continue;

                var rowModel = row.RowModel;
                var info = rowModel.Info;

                if (!LeagueGuidHelper.IsValidRawGuidHex(info.RawGuidHex))
                    continue;

                string rawGuid = LeagueGuidHelper.NormalizeRawGuidHex(info.RawGuidHex);

                plan.NewRegisteredGuids.Add(rawGuid);

                if (rowModel.IsMissingNonDefaultSave)
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