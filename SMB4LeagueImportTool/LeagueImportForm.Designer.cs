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
            tableLayoutPanelTop = new TableLayoutPanel();
            OpenSavesFolderButton = new Button();
            SelectSavePathButton = new Button();
            SavesFolderPathLabel = new Label();
            AboutButton = new Button();
            LoadLeaguesFranchisesButton = new Button();
            BottomPanel = new Panel();
            tableLayoutPanelBottom = new TableLayoutPanel();
            flowLayoutPanelActions = new FlowLayoutPanel();
            SaveChangesButton = new Button();
            ExportSaveButton = new Button();
            QuitButton = new Button();
            statusStrip1 = new StatusStrip();
            LeagueImportToolStatusLabel = new ToolStripStatusLabel();
            DGVLeaguePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVLeagues).BeginInit();
            UpperPanel.SuspendLayout();
            tableLayoutPanelTop.SuspendLayout();
            BottomPanel.SuspendLayout();
            tableLayoutPanelBottom.SuspendLayout();
            flowLayoutPanelActions.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // DGVLeaguePanel
            // 
            DGVLeaguePanel.Controls.Add(DGVLeagues);
            DGVLeaguePanel.Dock = DockStyle.Fill;
            DGVLeaguePanel.Location = new Point(0, 64);
            DGVLeaguePanel.Name = "DGVLeaguePanel";
            DGVLeaguePanel.Size = new Size(804, 374);
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
            DGVLeagues.Size = new Size(804, 374);
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
            UpperPanel.Controls.Add(tableLayoutPanelTop);
            UpperPanel.Dock = DockStyle.Top;
            UpperPanel.Location = new Point(0, 0);
            UpperPanel.Name = "UpperPanel";
            UpperPanel.Size = new Size(804, 64);
            UpperPanel.TabIndex = 1;
            // 
            // tableLayoutPanelTop
            // 
            tableLayoutPanelTop.ColumnCount = 3;
            tableLayoutPanelTop.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelTop.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelTop.Controls.Add(OpenSavesFolderButton, 0, 1);
            tableLayoutPanelTop.Controls.Add(SelectSavePathButton, 0, 0);
            tableLayoutPanelTop.Controls.Add(SavesFolderPathLabel, 1, 0);
            tableLayoutPanelTop.Controls.Add(AboutButton, 2, 0);
            tableLayoutPanelTop.Dock = DockStyle.Fill;
            tableLayoutPanelTop.Location = new Point(0, 0);
            tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            tableLayoutPanelTop.RowCount = 2;
            tableLayoutPanelTop.RowStyles.Add(new RowStyle());
            tableLayoutPanelTop.RowStyles.Add(new RowStyle());
            tableLayoutPanelTop.Size = new Size(804, 64);
            tableLayoutPanelTop.TabIndex = 0;
            // 
            // OpenSavesFolderButton
            // 
            OpenSavesFolderButton.Location = new Point(3, 35);
            OpenSavesFolderButton.Name = "OpenSavesFolderButton";
            OpenSavesFolderButton.Size = new Size(189, 24);
            OpenSavesFolderButton.TabIndex = 5;
            OpenSavesFolderButton.Text = "Open Saves Folder";
            OpenSavesFolderButton.UseVisualStyleBackColor = true;
            OpenSavesFolderButton.Click += OpenSavesFolderButton_Click;
            // 
            // SelectSavePathButton
            // 
            SelectSavePathButton.Location = new Point(3, 3);
            SelectSavePathButton.Name = "SelectSavePathButton";
            SelectSavePathButton.Size = new Size(189, 26);
            SelectSavePathButton.TabIndex = 3;
            SelectSavePathButton.Text = "Select SMB4 Saves Folder";
            SelectSavePathButton.UseVisualStyleBackColor = true;
            // 
            // SavesFolderPathLabel
            // 
            SavesFolderPathLabel.AutoEllipsis = true;
            SavesFolderPathLabel.Dock = DockStyle.Fill;
            SavesFolderPathLabel.Location = new Point(198, 0);
            SavesFolderPathLabel.Name = "SavesFolderPathLabel";
            SavesFolderPathLabel.Size = new Size(486, 32);
            SavesFolderPathLabel.TabIndex = 4;
            SavesFolderPathLabel.Text = "No Folder Selected";
            SavesFolderPathLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // AboutButton
            // 
            AboutButton.Anchor = AnchorStyles.Right;
            AboutButton.Location = new Point(690, 3);
            AboutButton.Name = "AboutButton";
            AboutButton.Size = new Size(111, 26);
            AboutButton.TabIndex = 1;
            AboutButton.Text = "About This Tool";
            AboutButton.UseVisualStyleBackColor = true;
            // 
            // LoadLeaguesFranchisesButton
            // 
            LoadLeaguesFranchisesButton.Anchor = AnchorStyles.Left;
            LoadLeaguesFranchisesButton.AutoSize = true;
            LoadLeaguesFranchisesButton.Location = new Point(3, 6);
            LoadLeaguesFranchisesButton.Name = "LoadLeaguesFranchisesButton";
            LoadLeaguesFranchisesButton.Size = new Size(189, 32);
            LoadLeaguesFranchisesButton.TabIndex = 2;
            LoadLeaguesFranchisesButton.Text = "Load All League/Franchise Saves";
            LoadLeaguesFranchisesButton.UseVisualStyleBackColor = true;
            // 
            // BottomPanel
            // 
            BottomPanel.Controls.Add(tableLayoutPanelBottom);
            BottomPanel.Controls.Add(statusStrip1);
            BottomPanel.Dock = DockStyle.Bottom;
            BottomPanel.Location = new Point(0, 438);
            BottomPanel.Name = "BottomPanel";
            BottomPanel.Size = new Size(804, 67);
            BottomPanel.TabIndex = 2;
            // 
            // tableLayoutPanelBottom
            // 
            tableLayoutPanelBottom.ColumnCount = 2;
            tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelBottom.Controls.Add(LoadLeaguesFranchisesButton, 0, 0);
            tableLayoutPanelBottom.Controls.Add(flowLayoutPanelActions, 1, 0);
            tableLayoutPanelBottom.Dock = DockStyle.Fill;
            tableLayoutPanelBottom.Location = new Point(0, 0);
            tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            tableLayoutPanelBottom.RowCount = 1;
            tableLayoutPanelBottom.RowStyles.Add(new RowStyle());
            tableLayoutPanelBottom.Size = new Size(804, 45);
            tableLayoutPanelBottom.TabIndex = 3;
            // 
            // flowLayoutPanelActions
            // 
            flowLayoutPanelActions.AutoSize = true;
            flowLayoutPanelActions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelActions.Controls.Add(SaveChangesButton);
            flowLayoutPanelActions.Controls.Add(ExportSaveButton);
            flowLayoutPanelActions.Controls.Add(QuitButton);
            flowLayoutPanelActions.Location = new Point(450, 3);
            flowLayoutPanelActions.Name = "flowLayoutPanelActions";
            flowLayoutPanelActions.Size = new Size(351, 38);
            flowLayoutPanelActions.TabIndex = 2;
            flowLayoutPanelActions.WrapContents = false;
            // 
            // SaveChangesButton
            // 
            SaveChangesButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SaveChangesButton.Location = new Point(3, 3);
            SaveChangesButton.Name = "SaveChangesButton";
            SaveChangesButton.Size = new Size(111, 32);
            SaveChangesButton.TabIndex = 5;
            SaveChangesButton.Text = "Save Changes";
            SaveChangesButton.UseVisualStyleBackColor = true;
            // 
            // ExportSaveButton
            // 
            ExportSaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ExportSaveButton.Location = new Point(120, 3);
            ExportSaveButton.Name = "ExportSaveButton";
            ExportSaveButton.Size = new Size(111, 32);
            ExportSaveButton.TabIndex = 6;
            ExportSaveButton.Text = "Export .sav file";
            ExportSaveButton.UseVisualStyleBackColor = true;
            // 
            // QuitButton
            // 
            QuitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            QuitButton.Location = new Point(237, 3);
            QuitButton.Name = "QuitButton";
            QuitButton.Size = new Size(111, 32);
            QuitButton.TabIndex = 4;
            QuitButton.Text = "Quit";
            QuitButton.UseVisualStyleBackColor = true;
            QuitButton.Click += QuitButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { LeagueImportToolStatusLabel });
            statusStrip1.Location = new Point(0, 45);
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
            Controls.Add(DGVLeaguePanel);
            Controls.Add(UpperPanel);
            Controls.Add(BottomPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(820, 544);
            Name = "LeagueImportForm";
            DGVLeaguePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGVLeagues).EndInit();
            UpperPanel.ResumeLayout(false);
            tableLayoutPanelTop.ResumeLayout(false);
            BottomPanel.ResumeLayout(false);
            BottomPanel.PerformLayout();
            tableLayoutPanelBottom.ResumeLayout(false);
            tableLayoutPanelBottom.PerformLayout();
            flowLayoutPanelActions.ResumeLayout(false);
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
        private Button OpenSavesFolderButton;
        private TableLayoutPanel tableLayoutPanelTop;
        private FlowLayoutPanel flowLayoutPanelActions;
        private TableLayoutPanel tableLayoutPanelBottom;
    }
}