using Microsoft.Data.Sqlite;

namespace SMB4LeagueImportTool.Core
{
    internal static class SqliteConnectionFactory
    {
        public static SqliteConnection CreateReadOnly(string sqlitePath)
        {
            if (string.IsNullOrWhiteSpace(sqlitePath))
                throw new ArgumentException("SQLite path cannot be empty.", nameof(sqlitePath));

            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = sqlitePath,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString();

            return new SqliteConnection(connectionString);
        }

        public static SqliteConnection CreateReadWrite(string sqlitePath)
        {
            if (string.IsNullOrWhiteSpace(sqlitePath))
                throw new ArgumentException("SQLite path cannot be empty.", nameof(sqlitePath));

            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = sqlitePath,
                Mode = SqliteOpenMode.ReadWrite,
                Pooling = false
            }.ToString();

            return new SqliteConnection(connectionString);
        }
    }
}