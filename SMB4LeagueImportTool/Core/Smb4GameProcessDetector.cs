using System.Diagnostics;

namespace SMB4LeagueImportTool.Core
{
    internal static class Smb4GameProcessDetector
    {
        public static bool TryFindRunningGame(out Smb4GameProcessInfo? processInfo)
        {
            processInfo = null;

            foreach (var process in Process.GetProcesses())
            {
                using (process)
                {
                    try
                    {
                        if (process.HasExited)
                            continue;

                        if (!IsSmb4ProcessName(process.ProcessName))
                            continue;

                        processInfo = new Smb4GameProcessInfo(
                            process.ProcessName,
                            process.Id);

                        return true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Process exited while we were checking it.
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        // Some processes may not be inspectable under Windows/Wine.
                    }
                }
            }

            return false;
        }

        private static bool IsSmb4ProcessName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
                return false;

            string normalized = Path.GetFileNameWithoutExtension(processName.Trim());

            return string.Equals(
                normalized,
                Smb4SaveConstants.GameProcessName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}