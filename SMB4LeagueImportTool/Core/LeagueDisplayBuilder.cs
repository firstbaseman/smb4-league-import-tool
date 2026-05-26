using SMB4LeagueImportTool.Models;

namespace SMB4LeagueImportTool.Core
{
    /// <summary>
    /// Converts raw registry + scanned league save data into display-ready rows,
    /// including ordering, counts, and the initial registered GUID baseline.
    /// </summary>
    public static class LeagueDisplayBuilder
    {
        public static LeagueDisplayBuildResult Build(
            IReadOnlyDictionary<string, LeagueRowInfo> leagueInfos,
            IReadOnlyList<string> registeredGuids)
        {
            ArgumentNullException.ThrowIfNull(leagueInfos);
            ArgumentNullException.ThrowIfNull(registeredGuids);

            var allRows = new List<LeagueRowViewModel>();
            var registeredGuidSet = new HashSet<string>(
                registeredGuids,
                StringComparer.OrdinalIgnoreCase);

            // 1. Registered GUIDs in order from master.sav.
            foreach (var rawGuid in registeredGuids)
            {
                if (!leagueInfos.TryGetValue(rawGuid, out var info))
                {
                    // master.sav references a GUID that has no matching league-*.sav file.
                    bool isDefaultMissingLeague = LeagueGuidHelper.IsDefaultLeagueGuidRaw(rawGuid);

                    info = new LeagueRowInfo
                    {
                        RawGuidHex = rawGuid,
                        DisplayGuid = LeagueGuidHelper.ToDisplayGuid(rawGuid),
                        Name = isDefaultMissingLeague
                            ? "(Default league – save file missing)"
                            : "(Missing save file)",
                        Kind = isDefaultMissingLeague
                            ? LeagueKind.Default
                            : LeagueKind.Custom,
                        SaveFileName = string.Empty
                    };
                }

                allRows.Add(new LeagueRowViewModel(info, isRegistered: true));
            }

            // 2. Unregistered league-*.sav files.
            foreach (var kvp in leagueInfos)
            {
                string key = kvp.Key;

                bool isRegistered =
                    key.Length == 32 &&
                    registeredGuidSet.Contains(key);

                if (isRegistered)
                    continue;

                var info = kvp.Value;
                allRows.Add(new LeagueRowViewModel(info, isRegistered: false));
            }

            var registeredDefaults = new List<LeagueRowViewModel>();
            var registeredCustoms = new List<LeagueRowViewModel>();
            var registeredFranchises = new List<LeagueRowViewModel>();
            var unregisteredCustoms = new List<LeagueRowViewModel>();
            var unregisteredFranchises = new List<LeagueRowViewModel>();
            var others = new List<LeagueRowViewModel>();

            foreach (var row in allRows)
            {
                bool isDefault = row.IsDefaultLeague;
                bool isCustom = row.IsCustomLeague;
                bool isFranchise = row.IsFranchise;

                if (row.IsRegistered)
                {
                    if (isDefault)
                        registeredDefaults.Add(row);
                    else if (isCustom)
                        registeredCustoms.Add(row);
                    else if (isFranchise)
                        registeredFranchises.Add(row);
                    else
                        others.Add(row);
                }
                else
                {
                    if (isCustom)
                        unregisteredCustoms.Add(row);
                    else if (isFranchise)
                        unregisteredFranchises.Add(row);
                    else
                        others.Add(row);
                }
            }

            var result = new LeagueDisplayBuildResult
            {
                DefaultCount = registeredDefaults.Count,

                CustomCount =
                    registeredCustoms.Count +
                    unregisteredCustoms.Count,

                FranchiseCount =
                    registeredFranchises.Count +
                    unregisteredFranchises.Count,

                InitialRegisteredCount =
                    registeredDefaults.Count +
                    registeredCustoms.Count +
                    registeredFranchises.Count
            };

            foreach (var rawGuid in registeredDefaults
                         .Concat(registeredCustoms)
                         .Concat(registeredFranchises)
                         .Select(row => row.Info.RawGuidHex)
                         .Where(g => !string.IsNullOrWhiteSpace(g)))
            {
                result.InitialRegisteredGuids.Add(rawGuid);
            }

            result.RowsInDisplayOrder.AddRange(registeredDefaults);
            result.RowsInDisplayOrder.AddRange(registeredCustoms);
            result.RowsInDisplayOrder.AddRange(registeredFranchises);
            result.RowsInDisplayOrder.AddRange(unregisteredCustoms);
            result.RowsInDisplayOrder.AddRange(unregisteredFranchises);
            result.RowsInDisplayOrder.AddRange(others);

            return result;
        }
    }
}