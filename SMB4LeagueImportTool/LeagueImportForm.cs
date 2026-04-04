// SMB4 League Import Tool - LeagueImportForm
// Handles reading/writing league registrations between master.sav and league-*.sav files.
// Uses zlib-compressed SQLite saves and classifies entries as Default / Custom / Franchise.
using SMB4LeagueImportTool.Core;
using SMB4LeagueImportTool.Models;
using System.Text;

namespace SMB4LeagueImportTool
{
    public partial class LeagueImportForm : Form
    {
        private string? _savesFolderPath;
        private bool _isDataLoaded;
        private bool _hasUnsavedChanges;
        private bool _isUpdatingGrid;
        private int _initialRegisteredCount;
        private bool _steamCloudWarningShown;
        private HashSet<string> _initialRegisteredGuids = new(StringComparer.OrdinalIgnoreCase);
        public LeagueImportForm()
        {
            InitializeComponent();
            AppLogger.WriteSessionHeader();
            tableLayoutPanelTop.SetRowSpan(SavesFolderPathLabel, 2);
            tableLayoutPanelTop.SetRowSpan(AboutButton, 1);

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
            OpenLogsButton.Click += OpenLogsButton_Click;

            DGVLeagues.CurrentCellDirtyStateChanged += DGVLeagues_CurrentCellDirtyStateChanged;
            DGVLeagues.CellValueChanged += DGVLeagues_CellValueChanged;
            DGVLeagues.DataError += DGVLeagues_DataError;

            Load += LeagueImportForm_Load;
            FormClosing += LeagueImportForm_FormClosing;


            LoadLeaguesFranchisesButton.Enabled = false;
            _isDataLoaded = false;
            UpdateUiState();
        }

        private void UpdateUiState()
        {
            ExportSaveButton.Enabled = _isDataLoaded;

            SaveChangesButton.Enabled =
                _isDataLoaded &&
                HasPendingRegistrationChanges();
        }

        // -------------------- lifecycle --------------------

        private void LeagueImportForm_Load(object? sender, EventArgs e)
        {
            var last = Properties.Settings.Default.LastSavesFolder;

            if (!Smb4SaveFolderValidator.IsValid(last))
            {
                // Nothing saved, folder missing, or not a valid SMB4 saves folder.
                return;
            }

            ApplySelectedSavesFolder(
                last,
                "Previous Folder Loaded. Click on \"Load All League/Franchise Saves\" to get started.",
                saveAsLastUsedFolder: false);
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
                "Ko-fi Support: https://ko-fi.com/firstbaseman/\n\n" +
               $"Log file:\n{AppLogger.LogFilePath}\n";

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

            var validation = Smb4SaveFolderValidator.Validate(selectedPath);

            if (!validation.IsValid)
            {
                MessageBox.Show(this,
                    validation.Message,
                    "Invalid Saves Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                LeagueImportToolStatusLabel.Text = validation.StatusText;
                return;
            }

            if (!ConfirmDiscardUnsavedChanges("select a different saves folder"))
                return;

            ApplySelectedSavesFolder(
                selectedPath,
                "Saves folder selected. Click on \"Load All League/Franchise Saves\" to get started.",
                saveAsLastUsedFolder: true);
        }
        private void OpenSavesFolderButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_savesFolderPath))
            {
                MessageBox.Show(this,
                    "Please select a valid SMB4 saves folder first.",
                    "No Saves Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                ShellFolderOpener.OpenExistingFolder(_savesFolderPath);
                AppLogger.Info($"Opened saves folder: {_savesFolderPath}");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to open saves folder.", ex);

                MessageBox.Show(this,
                    "The saves folder could not be opened.\n\n" + ex.Message,
                    "Open Saves Folder Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // Workflow for Save Changes:
        // 1. Validate that every registered non-default row has a backing league-*.sav file.
        // 2. Build the new ordered list of registered GUIDs from the grid.
        // 3. Confirm with the user (before/after counts).
        // 4. Ask MasterLeagueRegistry to rewrite t_league_savedatas in master.sav.

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

            var currentRows = GetCurrentGridLeagueRows();

            var changePlan = LeagueRegistrationChangePlanner.BuildPlan(
                currentRows,
                _initialRegisteredGuids);

            var newRegisteredGuids = changePlan.NewRegisteredGuids;
            var missingCheckedSaves = changePlan.MissingCheckedSaves;
            int newRegisteredCount = changePlan.NewRegisteredCount;

            if (!changePlan.HasChanges)
            {
                ShowNoChangesToSaveMessage();
                return;
            }

            if (ShowMissingCheckedSavesWarningIfNeeded(missingCheckedSaves))
                return;

            if (!ConfirmSaveChanges(newRegisteredCount))
                return;

            AppLogger.Info(
                $"Save confirmed. Registered before={_initialRegisteredCount}, after={newRegisteredCount}.");

            try
            {
                MasterLeagueRegistry.RewriteRegisteredGuids(
                    _savesFolderPath,
                    newRegisteredGuids);

                AppLogger.Info("Save operation completed successfully.");

                ApplySuccessfulSaveState(
                    newRegisteredGuids,
                    newRegisteredCount);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Save failed while updating master.sav.", ex);

                if (!TryHandleSqliteInitError(ex))
                {
                    MessageBox.Show(this,
                        "An error occurred while saving changes to master.sav:\n\n" + ex.Message,
                        "Save Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private void OpenLogsButton_Click(object? sender, EventArgs e)
        {
            try
            {
                AppLogger.Info("Open Logs Folder requested.");

                ShellFolderOpener.CreateAndOpenFolder(AppLogger.LogFolderPath);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to open logs folder.", ex);

                MessageBox.Show(this,
                    "The logs folder could not be opened.\n\n" + ex.Message,
                    "Open Logs Folder Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void LeagueImportForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!ConfirmDiscardUnsavedChanges("close the tool"))
            {
                e.Cancel = true;
                return;
            }

            CleanupTempFolder();
        }

        private void CleanupTempFolder()
        {
            TempCleanupService.CleanupForSavesFolder(_savesFolderPath);
        }
        private void LoadLeaguesFranchisesButton_Click(object? sender, EventArgs e)
        {
            if (_savesFolderPath is null)
            {
                AppLogger.Warning("Load requested but no saves folder is selected.");

                MessageBox.Show(this,
                    "Please select a valid SMB4 saves folder first.",
                    "No Saves Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmDiscardUnsavedChanges("reload leagues and franchises"))
                return;

            try
            {
                AppLogger.Info($"Load requested for saves folder: {_savesFolderPath}");
                LoadLeaguesAndFranchises(_savesFolderPath);
                AppLogger.Info("Load completed successfully.");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Load failed.", ex);

                if (!TryHandleSqliteInitError(ex))
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

            RunWithGridUpdatesSuppressed(() =>
            {
                DGVLeagues.Rows.Clear();
            });
            LeagueImportToolStatusLabel.Text = "Loading leagues and franchises…";
            AppLogger.Info($"Starting scan of saves folder: {savesFolderPath}");

            var validation = Smb4SaveFolderValidator.Validate(savesFolderPath);

            if (!validation.IsValid)
            {
                MessageBox.Show(this,
                    validation.Message,
                    "Invalid Saves Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                LoadLeaguesFranchisesButton.Enabled = false;
                LeagueImportToolStatusLabel.Text = validation.StatusText;
                return;
            }

            bool? repairFilenameMismatchesThisLoad = null;

            var loadResult = LeagueImportLoader.Load(
                savesFolderPath,
                mismatch => AskToRepairFilenameMismatch(
                    mismatch,
                    ref repairFilenameMismatchesThisLoad));

            if (!loadResult.HasLeagueSaveFiles || loadResult.DisplayBuild is null)
            {
                LeagueImportToolStatusLabel.Text = loadResult.StatusText;
                return;
            }

            ApplyLeagueDisplayBuild(loadResult.DisplayBuild);

            ShowFilenameRepairResults(
                loadResult.RenamedSaves,
                loadResult.SkippedRenames,
                loadResult.FailedRenames);
        }
        private void DGVLeagues_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingGrid)
                return;

            if (DGVLeagues.IsCurrentCellDirty)
                DGVLeagues.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DGVLeagues_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingGrid)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // Only care about the Registered checkbox column.
            if (e.ColumnIndex != ColRegistered.Index)
                return;

            var row = DGVLeagues.Rows[e.RowIndex];
            if (row.ReadOnly)
                return; // Ignore default leagues.

            _hasUnsavedChanges = HasPendingRegistrationChanges();

            LeagueImportToolStatusLabel.Text = _hasUnsavedChanges
                ? "Pending changes…"
                : "No pending changes.";

            UpdateUiState();
        }
        private void DGVLeagues_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            AppLogger.Error(
                $"DataGridView data error. Context={e.Context}, RowIndex={e.RowIndex}, ColumnIndex={e.ColumnIndex}",
                e.Exception);

            e.ThrowException = false;

            LeagueImportToolStatusLabel.Text =
                "Grid display error detected. Check the logs for details.";
        }

        // -------------------- helpers --------------------

        private bool HasPendingRegistrationChanges()
        {
            if (!_isDataLoaded)
                return false;

            var currentRows = GetCurrentGridLeagueRows();

            var changePlan = LeagueRegistrationChangePlanner.BuildPlan(
                currentRows,
                _initialRegisteredGuids);

            return changePlan.HasChanges;
        }
        private void RunWithGridUpdatesSuppressed(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            _isUpdatingGrid = true;

            try
            {
                action();
            }
            finally
            {
                _isUpdatingGrid = false;
            }
        }

        private void ApplyLeagueDisplayBuild(LeagueDisplayBuildResult displayBuild)
        {
            _initialRegisteredCount = displayBuild.InitialRegisteredCount;
            _initialRegisteredGuids = new HashSet<string>(
                displayBuild.InitialRegisteredGuids,
                StringComparer.OrdinalIgnoreCase);

            RunWithGridUpdatesSuppressed(() =>
            {
                DGVLeagues.Rows.Clear();

                foreach (var info in displayBuild.RowsInDisplayOrder)
                    AddLeagueRowToGrid(info);
            });

            LeagueImportToolStatusLabel.Text =
                $"All defaults loaded, {displayBuild.CustomCount} custom league(s) found, {displayBuild.FranchiseCount} franchise(s) found.";

            AppLogger.Info(
                $"Load summary: defaults={displayBuild.DefaultCount}, customs={displayBuild.CustomCount}, franchises={displayBuild.FranchiseCount}, registered={_initialRegisteredCount}.");

            _isDataLoaded = true;
            _hasUnsavedChanges = false;

            UpdateUiState();
        }
        private bool ConfirmDiscardUnsavedChanges(string actionDescription)
        {
            if (!HasPendingRegistrationChanges())
            {
                _hasUnsavedChanges = false;
                return true;
            }

            var choice = MessageBox.Show(this,
                "You have unsaved registration changes.\n\n" +
                $"If you continue, those changes will be discarded before you {actionDescription}.\n\n" +
                "Do you want to continue?",
                "Discard Unsaved Changes?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            bool shouldContinue = choice == DialogResult.Yes;

            if (shouldContinue)
                AppLogger.Warning($"User discarded unsaved changes to {actionDescription}.");

            return shouldContinue;
        }
        private bool TryGetSelectedLeagueInfoForExport(out LeagueRowInfo? info)
        {
            info = null;

            if (!_isDataLoaded || _savesFolderPath is null)
            {
                MessageBox.Show(this,
                    "Please load your leagues and franchises before exporting a save.",
                    "Nothing to Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            var row = DGVLeagues.CurrentRow;

            if (row is null || row.IsNewRow)
            {
                MessageBox.Show(this,
                    "Please select a league or franchise row to export.",
                    "No Row Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            if (row.Tag is not LeagueRowInfo rowInfo)
            {
                MessageBox.Show(this,
                    "The selected row does not have an associated save file.",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            info = rowInfo;
            return true;
        }
        private void ApplySelectedSavesFolder(
            string savesFolderPath,
            string statusText,
            bool saveAsLastUsedFolder)
        {
            _savesFolderPath = savesFolderPath;

            AppLogger.Info($"Using saves folder: {_savesFolderPath}");

            MaybeWarnSteamCloud(_savesFolderPath);

            SavesFolderPathLabel.Text = savesFolderPath;
            LeagueImportToolStatusLabel.Text = statusText;

            if (saveAsLastUsedFolder)
            {
                Properties.Settings.Default.LastSavesFolder = savesFolderPath;
                Properties.Settings.Default.Save();
            }

            LoadLeaguesFranchisesButton.Enabled = true;

            // Path is valid, but league data must still be loaded.
            _isDataLoaded = false;
            _hasUnsavedChanges = false;

            UpdateUiState();
        }
        private void ShowNoChangesToSaveMessage()
        {
            LeagueImportToolStatusLabel.Text = "No changes to save.";
            _hasUnsavedChanges = false;
            UpdateUiState();

            MessageBox.Show(this,
                "There are no changes to save.",
                "No Changes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private bool ConfirmSaveChanges(int newRegisteredCount)
        {
            var confirm = MessageBox.Show(this,
                "You are about to update your master.sav file.\n\n" +
                $"Registered Leagues/Franchises Before: {_initialRegisteredCount}\n" +
                $"Registered Leagues/Franchises After:  {newRegisteredCount}\n\n" +
                "Would you like to continue?",
                "Confirm Save Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return confirm == DialogResult.Yes;
        }
        private void ApplySuccessfulSaveState(
            IEnumerable<string> newRegisteredGuids,
            int newRegisteredCount)
        {
            _initialRegisteredCount = newRegisteredCount;
            _initialRegisteredGuids = new HashSet<string>(
                newRegisteredGuids,
                StringComparer.OrdinalIgnoreCase);

            _hasUnsavedChanges = false;

            LeagueImportToolStatusLabel.Text =
                "Saved changes successfully. A timestamped backup of master.sav was created.";

            UpdateUiState();
        }
        private bool ShowMissingCheckedSavesWarningIfNeeded(
            IReadOnlyList<LeagueRowInfo> missingCheckedSaves)
        {
            if (missingCheckedSaves.Count == 0)
                return false;

            var sb = new StringBuilder();

            sb.AppendLine("One or more checked entries do not have a matching league-*.sav file.");
            sb.AppendLine();
            sb.AppendLine("These entries cannot be safely registered because master.sav would reference saves that do not exist.");
            sb.AppendLine();
            sb.AppendLine("Missing checked saves:");
            sb.AppendLine();

            foreach (var info in missingCheckedSaves.OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase))
            {
                sb.AppendLine($"• {info.Name} ({info.DisplayGuid})");
            }

            MessageBox.Show(this,
                sb.ToString(),
                "Missing Save Files",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            LeagueImportToolStatusLabel.Text = "Save canceled. One or more checked saves are missing.";
            return true;
        }
        private void ShowFilenameRepairResults(
    IReadOnlyList<(string OldName, string NewName, string LeagueName)> renamedSaves,
    IReadOnlyList<(string OldName, string CorrectName, string LeagueName)> skippedRenames,
    IReadOnlyList<(string FileName, string Reason)> failedRenames)
        {
            if (renamedSaves.Count > 0)
            {
                var sb = new StringBuilder();

                sb.AppendLine("One or more league save files had filenames that didn't match");
                sb.AppendLine("their internal IDs. The tool normalized them so the game and");
                sb.AppendLine("other tools can recognize them reliably.");
                sb.AppendLine();
                sb.AppendLine("Renamed saves:");
                sb.AppendLine();

                foreach (var (oldName, newName, leagueName) in
                         renamedSaves.OrderBy(r => r.LeagueName, StringComparer.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"{oldName} → {newName}   ({leagueName})");
                }

                MessageBox.Show(
                    this,
                    sb.ToString(),
                    "League Save Filenames Normalized",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (skippedRenames.Count > 0)
            {
                var sb = new StringBuilder();

                sb.AppendLine("One or more league save files had filenames that did not match their internal IDs.");
                sb.AppendLine("You chose not to repair them during this load.");
                sb.AppendLine();
                sb.AppendLine("Skipped repairs:");
                sb.AppendLine();

                foreach (var (oldName, correctName, leagueName) in
                         skippedRenames.OrderBy(r => r.LeagueName, StringComparer.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"{oldName} → {correctName}   ({leagueName})");
                }

                MessageBox.Show(
                    this,
                    sb.ToString(),
                    "League Save Filename Repair Skipped",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (failedRenames.Count > 0)
            {
                var sb = new StringBuilder();

                sb.AppendLine("One or more league save files could not be renamed to match their internal IDs.");
                sb.AppendLine("These files were loaded as-is and may not register correctly.");
                sb.AppendLine();
                sb.AppendLine("Failed renames:");
                sb.AppendLine();

                foreach (var (fileName, reason) in
                         failedRenames.OrderBy(r => r.FileName, StringComparer.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"{fileName}: {reason}");
                }

                MessageBox.Show(
                    this,
                    sb.ToString(),
                    "League Save Rename Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        private void MaybeWarnSteamCloud(string savesFolderPath)
        {
            if (_steamCloudWarningShown)
                return;

            if (!SteamCloudDetector.IsDetected(savesFolderPath))
                return;

            _steamCloudWarningShown = true;

            MessageBox.Show(
                this,
                "Steam Cloud appears to be enabled for Super Mega Baseball 4.\n\n" +
                "If Steam Cloud syncs after you edit/register saves, your changes may be overwritten by the cloud saves.\n\n" +
                "Recommended:\n" +
                "• Disable Steam Cloud for SMB4 while using this tool, or\n" +
                "• Back up your saves first.",
                "Steam Cloud Detected",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        private bool TryHandleSqliteInitError(Exception ex)
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

            if (LeagueGuidHelper.IsDefaultLeagueGuidRaw(info.RawGuidHex))
            {
                row.ReadOnly = true;
                row.DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
                row.DefaultCellStyle.ForeColor = System.Drawing.Color.DimGray;
            }
        }
        private bool AskToRepairFilenameMismatch(
            LeagueFilenameMismatchInfo mismatch,
            ref bool? repairFilenameMismatchesThisLoad)
        {
            if (repairFilenameMismatchesThisLoad is null)
            {
                var choice = MessageBox.Show(
                    this,
                    "One or more league/franchise save files have filenames that do not match their internal IDs.\n\n" +
                    "This can happen when a save was copied, renamed, exported, or shared manually.\n\n" +
                    "Repairing the filename helps SMB4 and related tools recognize the save reliably.\n\n" +
                    "Do you want this tool to repair mismatched league save filenames now?\n\n" +
                    "A timestamped backup will be created if the target filename already exists.",
                    "Repair League Save Filenames?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                repairFilenameMismatchesThisLoad = choice == DialogResult.Yes;
            }

            return repairFilenameMismatchesThisLoad == true;
        }
        private List<LeagueRowInfo> GetCurrentGridLeagueRows()
        {
            var rows = new List<LeagueRowInfo>();

            foreach (DataGridViewRow row in DGVLeagues.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Tag is not LeagueRowInfo info)
                    continue;

                bool isRegistered = false;

                if (row.Cells[ColRegistered.Index].Value is bool checkedValue)
                    isRegistered = checkedValue;

                info.IsRegistered = isRegistered;
                rows.Add(info);
            }

            return rows;
        }
        private void ExportSaveButton_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedLeagueInfoForExport(out var info) || info is null)
                return;

            string savesFolderPath = _savesFolderPath!;
            string sourcePath;

            try
            {
                sourcePath = LeagueSaveExporter.GetSourcePath(savesFolderPath, info);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this,
                    ex.Message + "\n\nIt may be a missing or invalid registration.",
                    "No Save File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(this,
                    "The underlying save file could not be found on disk:\n\n" + ex.FileName,
                    "Save File Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Export source validation failed.", ex);

                MessageBox.Show(this,
                    "The selected save could not be prepared for export:\n\n" + ex.Message,
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Title = "Export league/franchise save",
                FileName = Path.GetFileName(sourcePath),
                Filter = "SMB4 Save Files (*.sav)|*.sav|All Files (*.*)|*.*",
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                LeagueSaveExporter.Export(
                    savesFolderPath,
                    info,
                    dialog.FileName);

                LeagueImportToolStatusLabel.Text =
                    $"Exported {info.Name} to {Path.GetFileName(dialog.FileName)}.";
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Export failed for destination path: {dialog.FileName}", ex);

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