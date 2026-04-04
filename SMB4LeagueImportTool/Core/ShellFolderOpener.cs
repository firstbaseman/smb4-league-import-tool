using System;
using System.Diagnostics;
using System.IO;

namespace SMB4LeagueImportTool.Core
{
    // Opens folders in Windows Explorer.
    // Keeps shell/process launching details out of the form.
    public static class ShellFolderOpener
    {
        public static void OpenExistingFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("Folder path cannot be empty.", nameof(folderPath));

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            OpenFolder(folderPath);
        }

        public static void CreateAndOpenFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("Folder path cannot be empty.", nameof(folderPath));

            Directory.CreateDirectory(folderPath);
            OpenFolder(folderPath);
        }

        private static void OpenFolder(string folderPath)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
        }
    }
}