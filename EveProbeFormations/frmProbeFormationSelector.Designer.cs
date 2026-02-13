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
            SuspendLayout();
            // 
            // listBoxSavedFormations
            // 
            listBoxSavedFormations.FormattingEnabled = true;
            listBoxSavedFormations.Location = new Point(12, 39);
            listBoxSavedFormations.Name = "listBoxSavedFormations";
            listBoxSavedFormations.Size = new Size(158, 259);
            listBoxSavedFormations.TabIndex = 0;
            listBoxSavedFormations.DoubleClick += listBoxSavedFormations_DoubleClick;
            // 
            // lblProfilePath
            // 
            lblProfilePath.AutoSize = true;
            lblProfilePath.Location = new Point(17, 13);
            lblProfilePath.Name = "lblProfilePath";
            lblProfilePath.Size = new Size(44, 15);
            lblProfilePath.TabIndex = 1;
            lblProfilePath.Text = "Profile:";
            // 
            // btnAddSegment
            // 
            btnAddSegment.Location = new Point(12, 333);
            btnAddSegment.Name = "btnAddSegment";
            btnAddSegment.Size = new Size(156, 23);
            btnAddSegment.TabIndex = 2;
            btnAddSegment.Text = "Import from Clipboard";
            btnAddSegment.UseVisualStyleBackColor = true;
            btnAddSegment.Click += btnAddSegment_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(12, 362);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(156, 23);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete Selected";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(12, 304);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(156, 23);
            btnExport.TabIndex = 4;
            btnExport.Text = "Export to Clipboard";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // frmProbeFormationSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(182, 397);
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
    }
}