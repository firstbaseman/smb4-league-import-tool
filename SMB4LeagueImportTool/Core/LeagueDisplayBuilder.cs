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

            var result = new LeagueDisplayBuildResult();

            var allInfos = new List<LeagueRowInfo>();
            var registeredGuidSet = new HashSet<string>(
                registeredGuids,
                StringComparer.OrdinalIgnoreCase);

            // 1. Registered GUIDs in order from master.sav.
            foreach (var rawGuid in registeredGuids)
            {
                if (!leagueInfos.TryGetValue(rawGuid, out var info))
                {
                    // master.sav references a GUID that has no matching league-*.sav file.
                    info = new LeagueRowInfo
                    {
                        RawGuidHex = rawGuid,
                        DisplayGuid = LeagueGuidHelper.FormatGuidWithDashes(rawGuid),
                        Name = LeagueGuidHelper.IsDefaultLeagueGuidRaw(rawGuid)
                            ? "(Default league – save file missing)"
                            : "(Missing save file)",
                        Type = LeagueGuidHelper.IsDefaultLeagueGuidRaw(rawGuid)
                        ? LeagueTypes.Default
                        : LeagueTypes.Custom,
                        SaveFileName = string.Empty
                    };
                }

                info.IsRegistered = true;
                allInfos.Add(info);
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
                info.IsRegistered = false;
                allInfos.Add(info);
            }

            var registeredDefaults = new List<LeagueRowInfo>();
            var registeredCustoms = new List<LeagueRowInfo>();
            var registeredFranchises = new List<LeagueRowInfo>();
            var unregisteredCustoms = new List<LeagueRowInfo>();
            var unregisteredFranchises = new List<LeagueRowInfo>();
            var others = new List<LeagueRowInfo>();

            foreach (var info in allInfos)
            {
                bool isDefault = string.Equals(info.Type, LeagueTypes.Default, StringComparison.OrdinalIgnoreCase);
                bool isCustom = string.Equals(info.Type, LeagueTypes.Custom, StringComparison.OrdinalIgnoreCase);
                bool isFranchise = string.Equals(info.Type, LeagueTypes.Franchise, StringComparison.OrdinalIgnoreCase);

                if (info.IsRegistered)
                {
                    if (isDefault)
                        registeredDefaults.Add(info);
                    else if (isCustom)
                        registeredCustoms.Add(info);
                    else if (isFranchise)
                        registeredFranchises.Add(info);
                    else
                        others.Add(info);
                }
                else
                {
                    if (isCustom)
                        unregisteredCustoms.Add(info);
                    else if (isFranchise)
                        unregisteredFranchises.Add(info);
                    else
                        others.Add(info);
                }
            }

            result.DefaultCount = registeredDefaults.Count;

            result.CustomCount =
                registeredCustoms.Count +
                unregisteredCustoms.Count;

            result.FranchiseCount =
                registeredFranchises.Count +
                unregisteredFranchises.Count;

            result.InitialRegisteredCount =
                registeredDefaults.Count +
                registeredCustoms.Count +
                registeredFranchises.Count;

            foreach (var rawGuid in registeredDefaults
                         .Concat(registeredCustoms)
                         .Concat(registeredFranchises)
                         .Select(i => i.RawGuidHex)
                         .Where(g => !string.IsNullOrWhiteSpace(g)))
            {
                result.InitialRegisteredGuids.Add(rawGuid);
            }

            AddBucket(result.RowsInDisplayOrder, registeredDefaults);
            AddBucket(result.RowsInDisplayOrder, registeredCustoms);
            AddBucket(result.RowsInDisplayOrder, registeredFranchises);
            AddBucket(result.RowsInDisplayOrder, unregisteredCustoms);
            AddBucket(result.RowsInDisplayOrder, unregisteredFranchises);
            AddBucket(result.RowsInDisplayOrder, others);

            return result;
        }

        private static void AddBucket(
            List<LeagueRowInfo> target,
            IEnumerable<LeagueRowInfo> bucket)
        {
            target.AddRange(bucket);
        }
    }
}