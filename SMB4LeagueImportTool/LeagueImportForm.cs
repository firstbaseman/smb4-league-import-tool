// SMB4 League Import Tool - LeagueImportForm
// Handles reading/writing league registrations between master.sav and league-*.sav files.
// Uses zlib-compressed SQLite saves and classifies entries as Default / Custom / Franchise.
using SMB4LeagueImportTool.Core;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows.Forms;

namespace SMB4LeagueImportTool
{
    public partial class LeagueImportForm : Form
    {
        private string? _savesFolderPath;
        private bool _isDataLoaded;
        private bool _hasUnsavedChanges;
        private int _initialRegisteredCount;

        // Raw hex (no dashes) as stored in t_league_savedatas.GUID
        private static readonly string[] DefaultLeagueGuidsRaw =
        {
            "99F30082775B4547ADD88C7D2C94FCE5",
            "1EE40D82453A474082E50827731C22E0",
            "7CBC32B9BD7F48D7AE0144C6595CD5A6"
        };
        // Flattened view model for a single league/franchise row in the grid.
        // Backed by data from master.sav (t_league_savedatas) and each league-*.sav file.
        private sealed class LeagueRowInfo
        {
            public string RawGuidHex { get; set; } = string.Empty;
            public string DisplayGuid { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public bool IsRegistered { get; set; }
            public string SaveFileName { get; set; } = string.Empty;
        }

        public LeagueImportForm()
        {
            InitializeComponent();

            // Initial UI text
            this.Text = $"SMB4 League Import Tool {VersionInfo.Version}";
            LeagueImportToolStatusLabel.Text = "Select your SMB4 saves folder to begin.";
            SavesFolderPathLabel.Text = "No folder selected";

            // Wire up events
            SelectSavePathButton.Click += SelectSavePathButton_Click;
            LoadLeaguesFranchisesButton.Click += LoadLeaguesFranchisesButton_Click;
            SaveChangesButton.Click += SaveChangesButton_Click;
            ExportSaveButton.Click += ExportSaveButton_Click;
            AboutButton.Click += AboutButton_Click;
            QuitButton.Click += QuitButton_Click;

            DGVLeagues.CurrentCellDirtyStateChanged += DGVLeagues_CurrentCellDirtyStateChanged;
            DGVLeagues.CellValueChanged += DGVLeagues_CellValueChanged;

            Load += LeagueImportForm_Load;
            FormClosing += LeagueImportForm_FormClosing;


            LoadLeaguesFranchisesButton.Enabled = false;
            _isDataLoaded = false;
            UpdateUiState();
        }

        private void UpdateUiState()
        {
            // Export/Save Changes only valid when data is loaded
            ExportSaveButton.Enabled = _isDataLoaded;
            SaveChangesButton.Enabled = _isDataLoaded;
        }

        // -------------------- lifecycle --------------------

        private void LeagueImportForm_Load(object? sender, EventArgs e)
        {
            var last = Properties.Settings.Default.LastSavesFolder;
            if (string.IsNullOrWhiteSpace(last) || !Directory.Exists(last))
            {
                // Nothing saved or folder missing; stay in initial state
                return;
            }

            var masterSavPath = Path.Combine(last, "master.sav");
            if (!File.Exists(masterSavPath))
            {
                // Folder exists but not a valid SMB4 saves folder
                return;
            }

            // Valid last-used saves folder
            _savesFolderPath = last;
            SavesFolderPathLabel.Text = last;
            LeagueImportToolStatusLabel.Text = "Previous folder loaded, let's begin";
            LoadLeaguesFranchisesButton.Enabled = true;
            _isDataLoaded = false;   // Path is good, but we haven't loaded leagues yet
            UpdateUiState();
        }

        // -------------------- UI handlers --------------------

        private void AboutButton_Click(object? sender, EventArgs e)
        {
            string message =
                $"{VersionInfo.FullVersion}\n\n" +
                "SMB4 LIT is a utility that allows you to import and register " +
                "custom leagues and franchises for Super Mega Baseball 4 so that " +
                "they appear properly in your game.\n\n" +
                "If someone shares a custom league or franchise with you, or if you " +
                "create multiple leagues yourself, this tool integrates them cleanly " +
                "into your own save structure.\n\n" +
                "Developed by Ari: https://github.com/firstbaseman/\n\n" +
                "Ko-fi Support: https://ko-fi.com/firstbaseman/\n";

            MessageBox.Show(
                message,
                "About SMB4 League Import Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void SelectSavePathButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select SMB4 Saves Folder",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            var selectedPath = dialog.SelectedPath;

            if (!Directory.Exists(selectedPath))
            {
                MessageBox.Show(this,
                    "The selected folder does not exist.",
                    "Folder Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var masterSavPath = Path.Combine(selectedPath, "master.sav");
            if (!File.Exists(masterSavPath))
            {
                MessageBox.Show(this,
                    "The selected folder does not contain master.sav.\n\n" +
                    "Please select your Super Mega Baseball 4 save folder.",
                    "master.sav Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _savesFolderPath = selectedPath;
            SavesFolderPathLabel.Text = selectedPath;
            LeagueImportToolStatusLabel.Text =
                "Saves folder selected. Click on \"Load All League/Franchise Saves\" to get started.";

            Properties.Settings.Default.LastSavesFolder = selectedPath;
            Properties.Settings.Default.Save();

            LoadLeaguesFranchisesButton.Enabled = true;
            _isDataLoaded = false;   // Path is good, but we haven't loaded leagues yet
            UpdateUiState();
        }
        // Workflow for Save Changes:
        // 1. Validate that every registered non-default row has a backing league-*.sav file.
        // 2. Build the new ordered list of registered GUIDs from the grid.
        // 3. Confirm with the user (before/after counts).
        // 4. Decompress master.sav, rewrite t_league_savedatas, and repack master.sav.

        private void SaveChangesButton_Click(object? sender, EventArgs e)
        {
            if (!_isDataLoaded || _savesFolderPath is null)
            {
                MessageBox.Show(this,
                    "Please load your leagues and franchises before saving changes.",
                    "Nothing to Save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Build the new registered GUID list from the grid
            var newRegisteredGuids = new List<string>();
            // --- Detect registered leagues whose save files are missing ---
            var missingSaveFiles = new List<string>();

            foreach (DataGridViewRow row in DGVLeagues.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool isRegistered = row.Cells[ColRegistered.Index].Value is bool b && b;
                if (!isRegistered)
                    continue;

                if (row.Tag is not LeagueRowInfo info)
                    continue;

                // Default leagues do NOT require external save files
                if (IsDefaultLeagueGuidRaw(info.RawGuidHex))
                    continue;

                // For non-defaults, ensure save file exists
                if (string.IsNullOrWhiteSpace(info.SaveFileName))
                {
                    missingSaveFiles.Add(info.Name);
                    continue;
                }

                string expectedPath = Path.Combine(_savesFolderPath, info.SaveFileName);
                if (!File.Exists(expectedPath))
                    missingSaveFiles.Add(info.Name);
            }

            if (missingSaveFiles.Count > 0)
            {
                string list = string.Join("\n", missingSaveFiles);

                var warn = MessageBox.Show(this,
                    "Warning: The following registered leagues/franchises do not have corresponding save files:\n\n" +
                    list +
                    "\n\nRegistering missing saves may cause issues in-game since there is no linked reference.\n\n" +
                    "Would you like to continue?",
                    "Missing Save Files",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (warn != DialogResult.Yes)
                    return;
            }

            foreach (DataGridViewRow row in DGVLeagues.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool isRegistered = row.Cells[ColRegistered.Index].Value is bool b && b;
                if (!isRegistered)
                    continue;

                if (row.Tag is not LeagueRowInfo info)
                    continue;

                if (string.IsNullOrWhiteSpace(info.RawGuidHex))
                    continue;

                newRegisteredGuids.Add(info.RawGuidHex.ToUpperInvariant());
            }

            int newRegisteredCount = newRegisteredGuids.Count;

            if (!_hasUnsavedChanges && newRegisteredCount == _initialRegisteredCount)
            {
                MessageBox.Show(this,
                    "There are no changes to save.",
                    "No Changes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(this,
                "You are about to update your master.sav file.\n\n" +
                $"Registered Leagues/Franchises Before: {_initialRegisteredCount}\n" +
                $"Registered Leagues/Franchises After:  {newRegisteredCount}\n\n" +
                "Would you like to continue?",
                "Confirm Save Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string masterSavPath = Path.Combine(_savesFolderPath, "master.sav");

                using var savManager = new SavManager(_savesFolderPath);
                string tempSqlitePath = savManager.DecompressSavToTemp(masterSavPath);

                using (var conn = new SqliteConnection(
                           $"Data Source={tempSqlitePath};Pooling=False;"))
                {
                    conn.Open();
                    using var tx = conn.BeginTransaction();

                    // Blow away existing rows and rewrite from our list in grid order
                    using (var deleteCmd = conn.CreateCommand())
                    {
                        deleteCmd.CommandText = "DELETE FROM t_league_savedatas;";
                        deleteCmd.Transaction = tx;
                        deleteCmd.ExecuteNonQuery();
                    }

                    using (var insertCmd = conn.CreateCommand())
                    {
                        insertCmd.CommandText =
                            "INSERT INTO t_league_savedatas (GUID, isMissing) VALUES (@guid, 0);";
                        insertCmd.Transaction = tx;

                        var guidParam = insertCmd.CreateParameter();
                        guidParam.ParameterName = "@guid";
                        guidParam.SqliteType = SqliteType.Blob;
                        insertCmd.Parameters.Add(guidParam);

                        foreach (var rawHex in newRegisteredGuids)
                        {
                            guidParam.Value = HexToBytes(rawHex);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }

                // Ensure no pooled connections are still holding the temp .sqlite file
                SqliteConnection.ClearAllPools();

                savManager.RepackTempSqliteToSav(tempSqlitePath, masterSavPath);

                _initialRegisteredCount = newRegisteredCount;
                _hasUnsavedChanges = false;
                LeagueImportToolStatusLabel.Text = "Saved changes successfully.";
            }
            catch (Exception ex)
            {
                if (ex is TypeInitializationException tie &&
                    tie.TypeName?.Contains("Microsoft.Data.Sqlite.SqliteConnection") == true)
                {
                    MessageBox.Show(this,
                        "The SQLite engine the tool uses failed to initialize.\n\n" +
                        "This usually happens when the EXE is run directly from inside the ZIP, " +
                        "or moved without the other files it shipped with.\n\n" +
                        "Please extract the ZIP to a folder and run the tool from there, " +
                        "without moving the EXE on its own.",
                        "SQLite Initialization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
        }
                else
                {
                    MessageBox.Show(this,
                        "An error occurred while saving changes to master.sav:\n\n" + ex.Message,
                        "Save Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
        }
    }

}
private void LeagueImportForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            CleanupTempFolder();
        }

        private void CleanupTempFolder()
        {
            try
            {
                // Make sure no pooled connections are still holding onto temp .sqlite files
                SqliteConnection.ClearAllPools();

                if (!string.IsNullOrWhiteSpace(_savesFolderPath))
                {
                    string tempFolder = Path.Combine(_savesFolderPath, "_smb4_temp");

                    if (Directory.Exists(tempFolder))
                    {
                        Directory.Delete(tempFolder, true);
                    }
                }
            }
            catch
            {
                // Silent fail — temp folder cleanup isn't critical
            }
        }
        private void LoadLeaguesFranchisesButton_Click(object? sender, EventArgs e)
        {
            if (_savesFolderPath is null)
            {
                MessageBox.Show(this,
                    "Please select a valid SMB4 saves folder first.",
                    "No Saves Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                LoadLeaguesAndFranchises(_savesFolderPath);
            }
            catch (Exception ex)
            {
                if (ex is TypeInitializationException tie &&
                    tie.TypeName?.Contains("Microsoft.Data.Sqlite.SqliteConnection") == true)
                {
                    MessageBox.Show(this,
                        "The SQLite engine the tool uses failed to initialize.\n\n" +
                        "This usually happens when the EXE is run directly from inside the ZIP, " +
                        "or moved without the other files it shipped with.\n\n" +
                        "Please extract the ZIP to a folder and run the tool from there, " +
                        "without moving the EXE on its own.",
                        "SQLite Initialization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(this,
                        $"An error occurred while loading your leagues and franchises:\n\n{ex.Message}",
                        "Error Loading Data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // Rebuilds the grid from disk:
        // - Decompresses master.sav and league-*.sav files
        // - Resolves each GUID to a name/type
        // - Marks default/custom/franchise + registered/unregistered
        // - Buckets rows into a stable, user-friendly display order

        private void LoadLeaguesAndFranchises(string savesFolderPath)
        {
            _isDataLoaded = false;
            UpdateUiState();

            DGVLeagues.Rows.Clear();
            LeagueImportToolStatusLabel.Text = "Loading leagues and franchises…";

            if (!Directory.Exists(savesFolderPath))
            {
                MessageBox.Show(this,
                    "The selected saves folder no longer exists.\n\nPlease re-select the folder.",
                    "Folder Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                LoadLeaguesFranchisesButton.Enabled = false;
                LeagueImportToolStatusLabel.Text = "Saves folder not found.";
                return;
            }

            string masterSavPath = Path.Combine(savesFolderPath, "master.sav");
            if (!File.Exists(masterSavPath))
            {
                MessageBox.Show(this,
                    "master.sav was not found in the selected folder.\n\n" +
                    "Please select the folder that contains your SMB4 save files.",
                    "master.sav Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                LoadLeaguesFranchisesButton.Enabled = false;
                LeagueImportToolStatusLabel.Text = "master.sav not found.";
                return;
            }

            string[] leagueSaveFiles = Directory.GetFiles(
                savesFolderPath,
                "league-*.sav",
                SearchOption.TopDirectoryOnly);

            if (leagueSaveFiles.Length == 0)
            {
                LeagueImportToolStatusLabel.Text =
                    "master.sav found, but no league-*.sav files were detected.";
                return;
            }

            var registeredGuids = new List<string>();
            var leagueInfos = new Dictionary<string, LeagueRowInfo>(StringComparer.OrdinalIgnoreCase);

            using (var savManager = new SavManager(savesFolderPath))
            {
                // --- Read registered GUIDs from master.sav ---
                string masterSqlitePath = savManager.DecompressSavToTemp(masterSavPath);
                using (var masterConn = new SqliteConnection($"Data Source={masterSqlitePath};Mode=ReadOnly;"))
                {
                    masterConn.Open();
                    using var cmd = new SqliteCommand(
                        "SELECT HEX(GUID), isMissing FROM t_league_savedatas ORDER BY rowid",
                        masterConn);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string rawHex = reader.IsDBNull(0) ? string.Empty : reader.GetString(0).ToUpperInvariant();
                        int isMissing = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                        if (string.IsNullOrWhiteSpace(rawHex))
                            continue;

                        // Game treats missing entries as invalid; we won't treat them as registered
                        if (isMissing != 0)
                            continue;

                        registeredGuids.Add(rawHex);
                    }
                }

                // --- Scan each league-*.sav file for name/guid/type ---
                foreach (var leagueSavPath in leagueSaveFiles)
                {
                    string fileName = Path.GetFileName(leagueSavPath);
                    string tempSqlitePath;

                    try
                    {
                        tempSqlitePath = savManager.DecompressSavToTemp(leagueSavPath);
                    }
                    catch
                    {
                        // If we can't decompress this .sav, still show a row so the user sees it
                        var brokenInfo = new LeagueRowInfo
                        {
                            RawGuidHex = string.Empty,
                            DisplayGuid = "N/A",
                            Name = Path.GetFileNameWithoutExtension(fileName) + " (failed to open)",
                            Type = "Unknown",
                            SaveFileName = fileName
                        };
                        leagueInfos[fileName] = brokenInfo;
                        continue;
                    }

                    string rawGuid = string.Empty;
                    string displayName = Path.GetFileNameWithoutExtension(fileName);
                    bool isFranchise = false;

                    using (var conn = new SqliteConnection($"Data Source={tempSqlitePath};Mode=ReadOnly;"))
                    {
                        conn.Open();

                        // Pull GUID + name from t_Leagues
                        using (var cmd = new SqliteCommand("SELECT HEX(GUID), name FROM t_Leagues LIMIT 1", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    rawGuid = reader.GetString(0).ToUpperInvariant();

                                if (!reader.IsDBNull(1))
                                    displayName = reader.GetString(1);
                            }
                        }

                        // Detect franchise via t_franchise_seasons
                        try
                        {
                            using var franchiseCmd =
                                new SqliteCommand("SELECT 1 FROM t_franchise_seasons LIMIT 1", conn);
                            using var fReader = franchiseCmd.ExecuteReader();
                            isFranchise = fReader.Read();
                        }
                        catch (SqliteException)
                        {
                            // Table may not exist in pure league saves; that's fine.
                            isFranchise = false;
                        }
                    }

                    string type;
                    if (!string.IsNullOrEmpty(rawGuid) && IsDefaultLeagueGuidRaw(rawGuid))
                        type = "Default";
                    else
                        type = isFranchise ? "Franchise" : "Custom";

                    var info = new LeagueRowInfo
                    {
                        RawGuidHex = rawGuid,
                        DisplayGuid = string.IsNullOrEmpty(rawGuid) ? "N/A" : FormatGuidWithDashes(rawGuid),
                        Name = displayName,
                        Type = type,
                        SaveFileName = fileName
                    };

                    // Use rawGuid as key when available; fall back to file name
                    string key = string.IsNullOrEmpty(rawGuid) ? fileName : rawGuid;
                    leagueInfos[key] = info;
                }
            }

            // --- Build unified list of LeagueRowInfo objects ---
            var allInfos = new List<LeagueRowInfo>();

            // HashSet for quick membership checks (case-insensitive)
            var registeredGuidSet = new HashSet<string>(registeredGuids, StringComparer.OrdinalIgnoreCase);

            // 1. Registered GUIDs in order from master.sav
            foreach (var rawGuid in registeredGuids)
            {
                if (!leagueInfos.TryGetValue(rawGuid, out var info))
                {
                    // master.sav references a GUID that has no corresponding league-*.sav file
                    info = new LeagueRowInfo
                    {
                        RawGuidHex = rawGuid,
                        DisplayGuid = FormatGuidWithDashes(rawGuid),
                        Name = IsDefaultLeagueGuidRaw(rawGuid)
                            ? "(Default league – save file missing)"
                            : "(Missing save file)",
                        Type = IsDefaultLeagueGuidRaw(rawGuid) ? "Default" : "Custom",
                        SaveFileName = string.Empty
                    };
                }

                info.IsRegistered = true;
                allInfos.Add(info);
            }

            // 2. Unregistered league-*.sav files (GUIDs not in t_league_savedatas)
            foreach (var kvp in leagueInfos)
            {
                string key = kvp.Key;

                // Keys that are 32-char hex are GUID-based keys
                bool isRegistered = key.Length == 32 && registeredGuidSet.Contains(key);
                if (isRegistered)
                    continue;

                var info = kvp.Value;
                if (!info.IsRegistered)
                    info.IsRegistered = false;

                allInfos.Add(info);
            }

            // Bucket LeagueRowInfo objects by registration + type so we can render the grid in a stable, human-readable order:
            // Registered Default
            // Registered Custom
            // Registered Franchise
            // Unregistered Custom
            // Unregistered Franchise
            // Other/Unknown (edge cases)

            var registeredDefaults = new List<LeagueRowInfo>();
            var registeredCustoms = new List<LeagueRowInfo>();
            var registeredFranchises = new List<LeagueRowInfo>();
            var unregisteredCustoms = new List<LeagueRowInfo>();
            var unregisteredFranchises = new List<LeagueRowInfo>();
            var others = new List<LeagueRowInfo>();

            foreach (var info in allInfos)
            {
                bool isDefault = string.Equals(info.Type, "Default", StringComparison.OrdinalIgnoreCase);
                bool isCustom = string.Equals(info.Type, "Custom", StringComparison.OrdinalIgnoreCase);
                bool isFranchise = string.Equals(info.Type, "Franchise", StringComparison.OrdinalIgnoreCase);

                if (info.IsRegistered)
                {
                    if (isDefault) registeredDefaults.Add(info);
                    else if (isCustom) registeredCustoms.Add(info);
                    else if (isFranchise) registeredFranchises.Add(info);
                    else others.Add(info);
                }
                else
                {
                    if (isCustom) unregisteredCustoms.Add(info);
                    else if (isFranchise) unregisteredFranchises.Add(info);
                    else others.Add(info);
                }
            }

            // --- Compute summary counts ---
            int defaultCount =
                registeredDefaults.Count;

            int customCount =
                registeredCustoms.Count +
                unregisteredCustoms.Count;

            int franchiseCount =
                registeredFranchises.Count +
                unregisteredFranchises.Count;

            // Track original registered count (used for the Save Changes dialog)
            _initialRegisteredCount =
                registeredDefaults.Count +
                registeredCustoms.Count +
                registeredFranchises.Count;

            // --- Populate the grid in the desired order ---

            DGVLeagues.Rows.Clear();

            void AddBucket(List<LeagueRowInfo> bucket)
            {
                foreach (var info in bucket)
                    AddLeagueRowToGrid(info);
            }

            AddBucket(registeredDefaults);
            AddBucket(registeredCustoms);
            AddBucket(registeredFranchises);
            AddBucket(unregisteredCustoms);
            AddBucket(unregisteredFranchises);
            AddBucket(others);

            LeagueImportToolStatusLabel.Text =
                $"All defaults loaded, {customCount} custom league(s) found, {franchiseCount} franchise(s) found.";

            _isDataLoaded = true;
            _hasUnsavedChanges = false;
            UpdateUiState();
        }

        private void DGVLeagues_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (DGVLeagues.IsCurrentCellDirty)
                DGVLeagues.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DGVLeagues_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Only care about the Registered checkbox column
            if (DGVLeagues.Columns[e.ColumnIndex] != ColRegistered)
                return;

            var row = DGVLeagues.Rows[e.RowIndex];
            if (row.ReadOnly)
                return; // ignore default leagues

            _hasUnsavedChanges = true;
            LeagueImportToolStatusLabel.Text = "Pending changes…";
        }

        // -------------------- helpers --------------------

        private static string FormatGuidWithDashes(string rawHex)
        {
            if (string.IsNullOrWhiteSpace(rawHex))
                return string.Empty;

            string upper = rawHex.ToUpperInvariant();
            if (upper.Length != 32)
                return upper;

            return string.Format("{0}-{1}-{2}-{3}-{4}",
                upper.Substring(0, 8),
                upper.Substring(8, 4),
                upper.Substring(12, 4),
                upper.Substring(16, 4),
                upper.Substring(20));
        }
        // Converts a 32-character hex GUID (as returned by HEX(GUID) in SQLite)
        // into the raw byte[] format expected by the GUID BLOB column in t_league_savedatas.
        private static byte[] HexToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Array.Empty<byte>();

            string cleaned = hex.Trim();
            if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring(2);

            if (cleaned.Length % 2 != 0)
                throw new ArgumentException("Hex string has an invalid length.", nameof(hex));

            var bytes = new byte[cleaned.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(cleaned.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
        private static bool IsDefaultLeagueGuidRaw(string rawHex)
        {
            if (string.IsNullOrEmpty(rawHex))
                return false;

            string upper = rawHex.ToUpperInvariant();
            foreach (var def in DefaultLeagueGuidsRaw)
            {
                if (upper == def)
                    return true;
            }

            return false;
        }

        private void AddLeagueRowToGrid(LeagueRowInfo info)
        {
            int rowIndex = DGVLeagues.Rows.Add(
                info.IsRegistered,
                info.Type,
                info.Name,
                info.DisplayGuid,
                info.SaveFileName);

            var row = DGVLeagues.Rows[rowIndex];

            row.Tag = info; // This is so Save/Export can reconstruct GUIDs and file names

            if (IsDefaultLeagueGuidRaw(info.RawGuidHex))
            {
                row.ReadOnly = true;
                row.DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
                row.DefaultCellStyle.ForeColor = System.Drawing.Color.DimGray;
            }
        }

        private void ExportSaveButton_Click(object? sender, EventArgs e)
        {
            if (!_isDataLoaded || _savesFolderPath is null)
            {
                MessageBox.Show(this,
                    "Please load your leagues and franchises before exporting a save.",
                    "Nothing to Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var row = DGVLeagues.CurrentRow;
            if (row is null || row.IsNewRow)
            {
                MessageBox.Show(this,
                    "Please select a league or franchise row to export.",
                    "No Row Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            // Each row should have a LeagueRowInfo in Tag.
            // If not, something went wrong during grid population.

            if (row.Tag is not LeagueRowInfo info)
            {
                MessageBox.Show(this,
                    "The selected row does not have an associated save file.",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(info.SaveFileName))
            {
                MessageBox.Show(this,
                    "This entry does not have a corresponding league-*.sav file to export.\n\n" +
                    "It may be a missing or invalid registration.",
                    "No Save File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string sourcePath = Path.Combine(_savesFolderPath, info.SaveFileName);
            if (!File.Exists(sourcePath))
            {
                MessageBox.Show(this,
                    "The underlying save file could not be found on disk:\n\n" + sourcePath,
                    "Save File Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Title = "Export league/franchise save",
                FileName = info.SaveFileName,
                Filter = "SMB4 Save Files (*.sav)|*.sav|All Files (*.*)|*.*",
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                File.Copy(sourcePath, dialog.FileName, overwrite: true);
                LeagueImportToolStatusLabel.Text = $"Exported {info.Name} to {Path.GetFileName(dialog.FileName)}.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "An error occurred while exporting the save file:\n\n" + ex.Message,
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void QuitButton_Click(object? sender, EventArgs e)
        {
            // Let the user know something is happening before we close
            LeagueImportToolStatusLabel.Text = "Doing some cleanup...";
            LeagueImportToolStatusLabel.Invalidate();
            Application.DoEvents();

            Close(); // triggers FormClosing → CleanupTempFolder()
        }
    }
}