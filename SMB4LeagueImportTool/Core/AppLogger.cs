using System.Text;

namespace SMB4LeagueImportTool.Core
{
    public static class AppLogger
    {
        private static readonly object LockObject = new();

        public static string LogFolderPath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SMB4LeagueImportTool",
            "Logs");

        public static string LogFilePath { get; } = Path.Combine(
            LogFolderPath,
            "SMB4LeagueImportTool.log");

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warning(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message, Exception? exception = null)
        {
            var sb = new StringBuilder();
            sb.Append(message);

            if (exception is not null)
            {
                sb.AppendLine();
                sb.AppendLine(exception.ToString());
            }

            Write("ERROR", sb.ToString());
        }

        public static void WriteSessionHeader()
        {
            Write("INFO", "------------------------------------------------------------");
            Write("INFO", "SMB4 League Import Tool session started");
            Write("INFO", $"Version: {VersionInfo.FullVersion}");
            Write("INFO", $"OS: {Environment.OSVersion}");
            Write("INFO", $".NET: {Environment.Version}");
            Write("INFO", $"Machine: {Environment.MachineName}");
            Write("INFO", $"User: {Environment.UserName}");
            Write("INFO", $"Log file: {LogFilePath}");
        }

        public static void TryOpenLogFolder()
        {
            try
            {
                Directory.CreateDirectory(LogFolderPath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = LogFolderPath,
                    UseShellExecute = true
                });
            }
            catch
            {
                // Intentionally ignored. Logging should never crash the app.
            }
        }

        private static void Write(string level, string message)
        {
            try
            {
                Directory.CreateDirectory(LogFolderPath);

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string line = $"[{timestamp}] [{level}] {message}{Environment.NewLine}";

                lock (LockObject)
                {
                    File.AppendAllText(LogFilePath, line, Encoding.UTF8);
                }
            }
            catch
            {
                // Logging must never become the thing that breaks the tool.
            }
        }
    }
}