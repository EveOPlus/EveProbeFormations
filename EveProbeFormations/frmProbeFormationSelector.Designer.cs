namespace EveProbeFormations
{
    partial class frmProbeFormationSelector
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
            listBoxSavedFormations = new ListBox();
            lblProfilePath = new Label();
            btnAddSegment = new Button();
            btnDelete = new Button();
            btnExport = new Button();
            lblAlias = new Label();
            txtAlias = new TextBox();
            SuspendLayout();
            // 
            // listBoxSavedFormations
            // 
            listBoxSavedFormations.FormattingEnabled = true;
            listBoxSavedFormations.Location = new Point(12, 65);
            listBoxSavedFormations.Name = "listBoxSavedFormations";
            listBoxSavedFormations.SelectionMode = SelectionMode.MultiExtended;
            listBoxSavedFormations.Size = new Size(158, 259);
            listBoxSavedFormations.TabIndex = 0;
            listBoxSavedFormations.DoubleClick += listBoxSavedFormations_DoubleClick;
            // 
            // lblProfilePath
            // 
            lblProfilePath.AutoSize = true;
            lblProfilePath.Location = new Point(12, 9);
            lblProfilePath.Name = "lblProfilePath";
            lblProfilePath.Size = new Size(44, 15);
            lblProfilePath.TabIndex = 1;
            lblProfilePath.Text = "Profile:";
            // 
            // btnAddSegment
            // 
            btnAddSegment.Location = new Point(12, 359);
            btnAddSegment.Name = "btnAddSegment";
            btnAddSegment.Size = new Size(156, 23);
            btnAddSegment.TabIndex = 2;
            btnAddSegment.Text = "Import from Clipboard";
            btnAddSegment.UseVisualStyleBackColor = true;
            btnAddSegment.Click += btnAddSegment_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(12, 388);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(156, 23);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete Selected";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(12, 330);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(156, 23);
            btnExport.TabIndex = 4;
            btnExport.Text = "Export to Clipboard";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // lblAlias
            // 
            lblAlias.AutoSize = true;
            lblAlias.Location = new Point(12, 33);
            lblAlias.Name = "lblAlias";
            lblAlias.Size = new Size(35, 15);
            lblAlias.TabIndex = 5;
            lblAlias.Text = "Alias:";
            // 
            // txtAlias
            // 
            txtAlias.Location = new Point(53, 30);
            txtAlias.Name = "txtAlias";
            txtAlias.Size = new Size(115, 23);
            txtAlias.TabIndex = 6;
            txtAlias.LostFocus += txtAlias_LostFocus;
            // 
            // frmProbeFormationSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(187, 426);
            Controls.Add(txtAlias);
            Controls.Add(lblAlias);
            Controls.Add(btnExport);
            Controls.Add(btnDelete);
            Controls.Add(btnAddSegment);
            Controls.Add(lblProfilePath);
            Controls.Add(listBoxSavedFormations);
            Name = "frmProbeFormationSelector";
            Text = "Probe Formation Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxSavedFormations;
        private Label lblProfilePath;
        private Button btnAddSegment;
        private Button btnDelete;
        private Button btnExport;
        private Label lblAlias;
        private TextBox txtAlias;
    }
}