namespace SMB4LeagueImportTool.Core
{
    internal sealed class Smb4GameProcessInfo
    {
        public Smb4GameProcessInfo(
            string processName,
            int processId)
        {
            ProcessName = processName;
            ProcessId = processId;
        }

        public string ProcessName { get; }

        public int ProcessId { get; }

        public string DisplayText =>
            $"{Smb4SaveConstants.GameExecutableFileName} (PID {ProcessId})";
    }
}