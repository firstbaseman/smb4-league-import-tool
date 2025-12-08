using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace SMB4LeagueImportTool.Core
{
    // Handles zlib compression and decompression between .sav and .sqlite files.
    // SMB4 .sav files are zlib-compressed SQLite databases.

    public static class SavCompression
    {
        // Decompresses a zlib-compressed .sav file into a plain .sqlite file.
        public static void DecompressSavToFile(string savPath, string sqlitePath)
        {
            if (savPath == null) throw new ArgumentNullException(nameof(savPath));
            if (sqlitePath == null) throw new ArgumentNullException(nameof(sqlitePath));
            if (!File.Exists(savPath))
                throw new FileNotFoundException("SAV file not found.", savPath);

            using (var input = File.OpenRead(savPath))
            using (var inflater = new InflaterInputStream(input))   // zlib -> raw
            using (var output = File.Create(sqlitePath))
            {
                inflater.CopyTo(output);
            }
        }

        // Compresses a plain .sqlite file into a zlib-compressed .sav file.
        public static void CompressSqliteToSav(string sqlitePath, string savPath)
        {
            if (sqlitePath == null) throw new ArgumentNullException(nameof(sqlitePath));
            if (savPath == null) throw new ArgumentNullException(nameof(savPath));
            if (!File.Exists(sqlitePath))
                throw new FileNotFoundException("SQLite file not found.", sqlitePath);

            using (var input = File.OpenRead(sqlitePath))
            using (var output = File.Create(savPath))
            using (var deflater = new DeflaterOutputStream(output)) // raw -> zlib
            {
                input.CopyTo(deflater);
                deflater.Finish(); // make sure everything is flushed
            }
        }
    }
}