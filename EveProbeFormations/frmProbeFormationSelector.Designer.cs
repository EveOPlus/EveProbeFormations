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
            btnAddSegment.Location = new Point(112, 307);
            btnAddSegment.Name = "btnAddSegment";
            btnAddSegment.Size = new Size(56, 23);
            btnAddSegment.TabIndex = 2;
            btnAddSegment.Text = "Clone";
            btnAddSegment.UseVisualStyleBackColor = true;
            btnAddSegment.Click += btnAddSegment_Click;
            // 
            // frmProbeFormationSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(182, 338);
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
    }
}