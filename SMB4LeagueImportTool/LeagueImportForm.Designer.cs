namespace SMB4LeagueImportTool
{
    partial class LeagueImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeagueImportForm));
            DGVLeaguePanel = new Panel();
            DGVLeagues = new DataGridView();
            ColRegistered = new DataGridViewCheckBoxColumn();
            ColType = new DataGridViewTextBoxColumn();
            ColLeagueFranchiseName = new DataGridViewTextBoxColumn();
            ColGUID = new DataGridViewTextBoxColumn();
            ColFileName = new DataGridViewTextBoxColumn();
            UpperPanel = new Panel();
            SavesFolderPathLabel = new Label();
            SelectSavePathButton = new Button();
            LoadLeaguesFranchisesButton = new Button();
            AboutButton = new Button();
            BottomPanel = new Panel();
            ExportSaveButton = new Button();
            SaveChangesButton = new Button();
            QuitButton = new Button();
            statusStrip1 = new StatusStrip();
            LeagueImportToolStatusLabel = new ToolStripStatusLabel();
            DGVLeaguePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVLeagues).BeginInit();
            UpperPanel.SuspendLayout();
            BottomPanel.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // DGVLeaguePanel
            // 
            DGVLeaguePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVLeaguePanel.Controls.Add(DGVLeagues);
            DGVLeaguePanel.Location = new Point(0, 70);
            DGVLeaguePanel.Name = "DGVLeaguePanel";
            DGVLeaguePanel.Size = new Size(805, 350);
            DGVLeaguePanel.TabIndex = 0;
            // 
            // DGVLeagues
            // 
            DGVLeagues.AllowUserToAddRows = false;
            DGVLeagues.AllowUserToDeleteRows = false;
            DGVLeagues.AllowUserToResizeColumns = false;
            DGVLeagues.AllowUserToResizeRows = false;
            DGVLeagues.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVLeagues.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DGVLeagues.BackgroundColor = SystemColors.Control;
            DGVLeagues.BorderStyle = BorderStyle.None;
            DGVLeagues.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGVLeagues.Columns.AddRange(new DataGridViewColumn[] { ColRegistered, ColType, ColLeagueFranchiseName, ColGUID, ColFileName });
            DGVLeagues.Dock = DockStyle.Fill;
            DGVLeagues.EditMode = DataGridViewEditMode.EditOnEnter;
            DGVLeagues.Location = new Point(0, 0);
            DGVLeagues.MultiSelect = false;
            DGVLeagues.Name = "DGVLeagues";
            DGVLeagues.RowHeadersVisible = false;
            DGVLeagues.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVLeagues.Size = new Size(805, 350);
            DGVLeagues.TabIndex = 1;
            // 
            // ColRegistered
            // 
            ColRegistered.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ColRegistered.DataPropertyName = "IsRegistered";
            ColRegistered.HeaderText = "Registered";
            ColRegistered.Name = "ColRegistered";
            ColRegistered.Resizable = DataGridViewTriState.True;
            ColRegistered.SortMode = DataGridViewColumnSortMode.Automatic;
            ColRegistered.Width = 87;
            // 
            // ColType
            // 
            ColType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ColType.DataPropertyName = "SaveType";
            ColType.FillWeight = 50F;
            ColType.HeaderText = "Type";
            ColType.Name = "ColType";
            ColType.ReadOnly = true;
            ColType.SortMode = DataGridViewColumnSortMode.NotSortable;
            ColType.Width = 38;
            // 
            // ColLeagueFranchiseName
            // 
            ColLeagueFranchiseName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColLeagueFranchiseName.DataPropertyName = "LeagueFranchiseName";
            ColLeagueFranchiseName.FillWeight = 60F;
            ColLeagueFranchiseName.HeaderText = "League/Franchise Name";
            ColLeagueFranchiseName.MaxInputLength = 24;
            ColLeagueFranchiseName.Name = "ColLeagueFranchiseName";
            ColLeagueFranchiseName.ReadOnly = true;
            ColLeagueFranchiseName.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // ColGUID
            // 
            ColGUID.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColGUID.DataPropertyName = "LeagueGUID";
            ColGUID.HeaderText = "GUID (league-*.sav)";
            ColGUID.Name = "ColGUID";
            ColGUID.ReadOnly = true;
            ColGUID.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // ColFileName
            // 
            ColFileName.DataPropertyName = "FileName";
            ColFileName.HeaderText = "File Name";
            ColFileName.Name = "ColFileName";
            ColFileName.Visible = false;
            // 
            // UpperPanel
            // 
            UpperPanel.Controls.Add(SavesFolderPathLabel);
            UpperPanel.Controls.Add(SelectSavePathButton);
            UpperPanel.Controls.Add(LoadLeaguesFranchisesButton);
            UpperPanel.Controls.Add(AboutButton);
            UpperPanel.Dock = DockStyle.Top;
            UpperPanel.Location = new Point(0, 0);
            UpperPanel.Name = "UpperPanel";
            UpperPanel.Size = new Size(804, 72);
            UpperPanel.TabIndex = 1;
            // 
            // SavesFolderPathLabel
            // 
            SavesFolderPathLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            SavesFolderPathLabel.AutoEllipsis = true;
            SavesFolderPathLabel.Location = new Point(207, 16);
            SavesFolderPathLabel.Name = "SavesFolderPathLabel";
            SavesFolderPathLabel.Size = new Size(468, 19);
            SavesFolderPathLabel.TabIndex = 4;
            SavesFolderPathLabel.Text = "No Folder Selected";
            // 
            // SelectSavePathButton
            // 
            SelectSavePathButton.Location = new Point(12, 12);
            SelectSavePathButton.Name = "SelectSavePathButton";
            SelectSavePathButton.Size = new Size(189, 23);
            SelectSavePathButton.TabIndex = 3;
            SelectSavePathButton.Text = "Select SMB4 Saves Folder";
            SelectSavePathButton.UseVisualStyleBackColor = true;
            // 
            // LoadLeaguesFranchisesButton
            // 
            LoadLeaguesFranchisesButton.Location = new Point(12, 41);
            LoadLeaguesFranchisesButton.Name = "LoadLeaguesFranchisesButton";
            LoadLeaguesFranchisesButton.Size = new Size(189, 23);
            LoadLeaguesFranchisesButton.TabIndex = 2;
            LoadLeaguesFranchisesButton.Text = "Load All League/Franchise Saves";
            LoadLeaguesFranchisesButton.UseVisualStyleBackColor = true;
            // 
            // AboutButton
            // 
            AboutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AboutButton.Location = new Point(681, 12);
            AboutButton.Name = "AboutButton";
            AboutButton.Size = new Size(111, 23);
            AboutButton.TabIndex = 1;
            AboutButton.Text = "About This Tool";
            AboutButton.UseVisualStyleBackColor = true;
            // 
            // BottomPanel
            // 
            BottomPanel.Controls.Add(ExportSaveButton);
            BottomPanel.Controls.Add(SaveChangesButton);
            BottomPanel.Controls.Add(QuitButton);
            BottomPanel.Controls.Add(statusStrip1);
            BottomPanel.Dock = DockStyle.Bottom;
            BottomPanel.Location = new Point(0, 401);
            BottomPanel.Name = "BottomPanel";
            BottomPanel.Size = new Size(804, 104);
            BottomPanel.TabIndex = 2;
            // 
            // ExportSaveButton
            // 
            ExportSaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ExportSaveButton.Location = new Point(681, 25);
            ExportSaveButton.Name = "ExportSaveButton";
            ExportSaveButton.Size = new Size(111, 23);
            ExportSaveButton.TabIndex = 6;
            ExportSaveButton.Text = "Export .sav file";
            ExportSaveButton.UseVisualStyleBackColor = true;
            // 
            // SaveChangesButton
            // 
            SaveChangesButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SaveChangesButton.Location = new Point(564, 54);
            SaveChangesButton.Name = "SaveChangesButton";
            SaveChangesButton.Size = new Size(111, 23);
            SaveChangesButton.TabIndex = 5;
            SaveChangesButton.Text = "Save Changes";
            SaveChangesButton.UseVisualStyleBackColor = true;
            // 
            // QuitButton
            // 
            QuitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            QuitButton.Location = new Point(681, 54);
            QuitButton.Name = "QuitButton";
            QuitButton.Size = new Size(111, 23);
            QuitButton.TabIndex = 4;
            QuitButton.Text = "Quit";
            QuitButton.UseVisualStyleBackColor = true;
            QuitButton.Click += QuitButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { LeagueImportToolStatusLabel });
            statusStrip1.Location = new Point(0, 82);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(804, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // LeagueImportToolStatusLabel
            // 
            LeagueImportToolStatusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LeagueImportToolStatusLabel.Name = "LeagueImportToolStatusLabel";
            LeagueImportToolStatusLabel.Size = new Size(41, 17);
            LeagueImportToolStatusLabel.Text = "Ready";
            // 
            // LeagueImportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 505);
            Controls.Add(UpperPanel);
            Controls.Add(DGVLeaguePanel);
            Controls.Add(BottomPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(820, 544);
            Name = "LeagueImportForm";
            DGVLeaguePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGVLeagues).EndInit();
            UpperPanel.ResumeLayout(false);
            BottomPanel.ResumeLayout(false);
            BottomPanel.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel DGVLeaguePanel;
        private Panel UpperPanel;
        private Button LoadLeaguesFranchisesButton;
        private Button AboutButton;
        private Panel BottomPanel;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel LeagueImportToolStatusLabel;
        private Button ExportSaveButton;
        private Button QuitButton;
        private Label SavesFolderPathLabel;
        private Button SelectSavePathButton;
        private Button SaveChangesButton;
        private DataGridView DGVLeagues;
        private DataGridViewCheckBoxColumn ColRegistered;
        private DataGridViewTextBoxColumn ColType;
        private DataGridViewTextBoxColumn ColLeagueFranchiseName;
        private DataGridViewTextBoxColumn ColGUID;
        private DataGridViewTextBoxColumn ColFileName;
    }
}