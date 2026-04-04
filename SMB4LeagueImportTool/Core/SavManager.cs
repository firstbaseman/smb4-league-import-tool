namespace SMB4LeagueImportTool.Core
{
    /// <summary>
    /// Manages temporary SQLite files decompressed from .sav files,
    /// and safely repacks edited SQLite files back into .sav files.
    /// </summary>
    public sealed class SavManager : IDisposable
    {
        private readonly string _tempRoot;
        private readonly List<string> _tempFiles = new();
        private bool _disposed;

        public SavManager(string savesFolderPath)
        {
            ArgumentNullException.ThrowIfNull(savesFolderPath);

            if (!Directory.Exists(savesFolderPath))
                throw new DirectoryNotFoundException($"Saves folder not found: {savesFolderPath}");

            string appTempRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SMB4LeagueImportTool",
                "Temp");

            _tempRoot = Path.Combine(appTempRoot, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempRoot);
        }

        /// <summary>
        /// Decompresses a .sav file into a temporary .sqlite file and returns the path.
        /// </summary>
        public string DecompressSavToTemp(string savPath)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(savPath);

            if (!File.Exists(savPath))
                throw new FileNotFoundException("SAV file not found.", savPath);

            string baseName = Path.GetFileNameWithoutExtension(savPath);

            string tempSqlite = Path.Combine(
                _tempRoot,
                $"{baseName}-{Guid.NewGuid():N}.sqlite");

            SavCompression.DecompressSavToFile(savPath, tempSqlite);

            _tempFiles.Add(tempSqlite);
            return tempSqlite;
        }

        /// <summary>
        /// Safely recompresses the given temp .sqlite file back into the target .sav file.
        /// The target save is not touched until compression succeeds.
        /// A timestamped .bak file is created when replacing an existing save.
        /// </summary>
        public void RepackTempSqliteToSav(string tempSqlitePath, string targetSavPath)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(tempSqlitePath);
            ArgumentNullException.ThrowIfNull(targetSavPath);

            if (!File.Exists(tempSqlitePath))
                throw new FileNotFoundException("Temporary SQLite file not found.", tempSqlitePath);

            string targetFullPath = Path.GetFullPath(targetSavPath);
            string? targetFolder = Path.GetDirectoryName(targetFullPath);

            if (string.IsNullOrWhiteSpace(targetFolder))
                throw new InvalidOperationException("Could not determine target SAV folder.");

            Directory.CreateDirectory(targetFolder);

            string targetFileName = Path.GetFileName(targetFullPath);
            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");

            string tempSavPath = Path.Combine(
                targetFolder,
                $"{targetFileName}.{Guid.NewGuid():N}.tmp");

            string backupPath = Path.Combine(
                targetFolder,
                $"{targetFileName}.{timestamp}.bak");

            try
            {
                SavCompression.CompressSqliteToSav(tempSqlitePath, tempSavPath);

                if (File.Exists(targetFullPath))
                {
                    File.Replace(
                        tempSavPath,
                        targetFullPath,
                        backupPath,
                        ignoreMetadataErrors: true);
                }
                else
                {
                    File.Move(tempSavPath, targetFullPath);
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(tempSavPath))
                        File.Delete(tempSavPath);
                }
                catch
                {
                    // Best-effort cleanup only.
                }
            }
        }

        /// <summary>
        /// Explicit temp cleanup if caller wants to delete files before disposal.
        /// </summary>
        public void CleanupTempFolder()
        {
            if (_disposed)
                return;

            foreach (var file in _tempFiles)
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch
                {
                    // Best-effort cleanup only.
                }
            }

            _tempFiles.Clear();

            try
            {
                if (Directory.Exists(_tempRoot))
                    Directory.Delete(_tempRoot, recursive: true);
            }
            catch
            {
                // Best-effort cleanup only.
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            CleanupTempFolder();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SavManager));
        }
    }
}