using System;
using System.Collections.Generic;
using System.IO;

namespace SMB4LeagueImportTool.Core
{
    /// Manages temporary SQLite files decompressed from .sav files,
    /// and cleans them up when disposed.
    public sealed class SavManager : IDisposable
    {
        private readonly string _tempRoot;
        private readonly List<string> _tempFiles = new();

        public SavManager(string savesFolderPath)
        {
            ArgumentNullException.ThrowIfNull(savesFolderPath);

            // Temp folder inside the SMB4 saves directory
            _tempRoot = Path.Combine(savesFolderPath, "_smb4_temp");

            Directory.CreateDirectory(_tempRoot);
        }
        /// Decompresses a .sav file into a temporary .sqlite file and returns the path.
        public string DecompressSavToTemp(string savPath)
        {
            ArgumentNullException.ThrowIfNull(savPath);

            if (!File.Exists(savPath))
                throw new FileNotFoundException("SAV file not found.", savPath);

            // Use the file name without .sav to create a temp .sqlite file
            string baseName = Path.GetFileNameWithoutExtension(savPath);

            // Unique temp filename avoids collisions
            string tempSqlite = Path.Combine(
                _tempRoot,
                $"{baseName}-{Guid.NewGuid():N}.sqlite");

            // SMB4 decompression (external helper)
            SavCompression.DecompressSavToFile(savPath, tempSqlite);

            _tempFiles.Add(tempSqlite);
            return tempSqlite;
        }

        /// Re-compresses the given temp .sqlite file back into the target .sav file.
        /// The caller ensures no SQLite connections are open on tempSqlitePath.
        public void RepackTempSqliteToSav(string tempSqlitePath, string targetSavPath)
        {
            ArgumentNullException.ThrowIfNull(tempSqlitePath);
            ArgumentNullException.ThrowIfNull(targetSavPath);

            SavCompression.CompressSqliteToSav(tempSqlitePath, targetSavPath);
        }
        /// Explicit temp cleanup if caller wants to delete files before disposal.
        public void CleanupTempFolder()
        {
            foreach (var file in _tempFiles)
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch
                {
                    // ignore individual file delete failures
                }
            }

            _tempFiles.Clear();

            try
            {
                if (Directory.Exists(_tempRoot) &&
                    Directory.GetFileSystemEntries(_tempRoot).Length == 0)
                {
                    Directory.Delete(_tempRoot);
                }
            }
            catch
            {
                // ignore folder delete failures
            }
        }
        public void Dispose()
        {
            CleanupTempFolder();
            GC.SuppressFinalize(this);
        }
    }
}
