using SMB4LeagueImportTool.Core;

namespace SMB4LeagueImportTool
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new LeagueImportForm());
        }

        private static void Application_ThreadException(
            object sender,
            ThreadExceptionEventArgs e)
        {
            AppLogger.Error("Unhandled UI thread exception.", e.Exception);

            MessageBox.Show(
                "An unexpected error occurred.\n\n" +
                "The error was written to the log file. Please check the logs folder for details.\n\n" +
                e.Exception.Message,
                "Unexpected Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                AppLogger.Error("Unhandled application exception.", ex);
            }
            else
            {
                AppLogger.Error(
                    "Unhandled application exception. Exception object was not a System.Exception.");
            }
        }
    }
}