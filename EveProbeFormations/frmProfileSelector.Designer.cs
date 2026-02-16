namespace EveProbeFormations
{
    partial class frmProfileSelector
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnRefreshSettingsPath = new Button();
            listBoxUserDatPaths = new ListBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            panel2 = new Panel();
            label1 = new Label();
            panel1 = new Panel();
            btnSettingsFolderPicker = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnRefreshSettingsPath
            // 
            btnRefreshSettingsPath.Location = new Point(107, 0);
            btnRefreshSettingsPath.Name = "btnRefreshSettingsPath";
            btnRefreshSettingsPath.Size = new Size(58, 28);
            btnRefreshSettingsPath.TabIndex = 1;
            btnRefreshSettingsPath.Text = "Refresh";
            btnRefreshSettingsPath.UseVisualStyleBackColor = true;
            btnRefreshSettingsPath.Click += btnRefreshSettingsPath_Click;
            // 
            // listBoxUserDatPaths
            // 
            listBoxUserDatPaths.Dock = DockStyle.Fill;
            listBoxUserDatPaths.FormattingEnabled = true;
            listBoxUserDatPaths.Location = new Point(3, 43);
            listBoxUserDatPaths.Name = "listBoxUserDatPaths";
            listBoxUserDatPaths.Size = new Size(731, 530);
            listBoxUserDatPaths.TabIndex = 2;
            listBoxUserDatPaths.DoubleClick += listBoxUserDatPaths_DoubleClick;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(listBoxUserDatPaths, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(737, 576);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 177F));
            tableLayoutPanel2.Controls.Add(panel2, 0, 0);
            tableLayoutPanel2.Controls.Add(panel1, 1, 0);
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(731, 34);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(548, 28);
            panel2.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 6);
            label1.Name = "label1";
            label1.Size = new Size(524, 15);
            label1.TabIndex = 0;
            label1.Text = "Double click on the Eve profile to manage. If the list is empty, try to manually set the profile folder.";
            // 
            // panel1
            // 
            panel1.Controls.Add(btnSettingsFolderPicker);
            panel1.Controls.Add(btnRefreshSettingsPath);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(557, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(171, 28);
            panel1.TabIndex = 1;
            // 
            // btnSettingsFolderPicker
            // 
            btnSettingsFolderPicker.Location = new Point(3, 0);
            btnSettingsFolderPicker.Name = "btnSettingsFolderPicker";
            btnSettingsFolderPicker.Size = new Size(98, 28);
            btnSettingsFolderPicker.TabIndex = 2;
            btnSettingsFolderPicker.Text = "Manually Set";
            btnSettingsFolderPicker.UseVisualStyleBackColor = true;
            btnSettingsFolderPicker.Click += btnSettingsFolderPicker_Click;
            // 
            // frmProfileSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(737, 576);
            Controls.Add(tableLayoutPanel1);
            Name = "frmProfileSelector";
            Text = "Select User Profile";
            Load += frmProfileSelector_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnRefreshSettingsPath;
        private ListBox listBoxUserDatPaths;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel2;
        private Panel panel1;
        private Button btnSettingsFolderPicker;
        private Label label1;
    }
}
