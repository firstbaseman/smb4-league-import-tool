namespace SMB4LeagueImportTool.Core
{
    public static class Smb4SqlQueries
    {
        public const string ReadRegisteredLeagueGuids =
            "SELECT HEX(GUID), isMissing FROM t_league_savedatas ORDER BY rowid";

        public const string DeleteRegisteredLeagueGuids =
            "DELETE FROM t_league_savedatas;";

        public const string InsertRegisteredLeagueGuid =
            "INSERT INTO t_league_savedatas (GUID, isMissing) VALUES (@guid, 0);";

        public const string ReadLeagueGuidAndName =
            "SELECT HEX(GUID), name FROM t_Leagues LIMIT 1";

        public const string DetectFranchiseSave =
            "SELECT 1 FROM t_franchise LIMIT 1";
    }
}