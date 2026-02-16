using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EveProbeFormations
{
    public partial class frmProbeFormationSelector : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PathToUserProfile { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UserProfileProcessor UserProfileProcessor { get; set; }

        public frmProbeFormationSelector(string pathToUserProfile)
        {
            PathToUserProfile = pathToUserProfile;
            InitializeComponent();

            var profileName = Path.GetFileName(pathToUserProfile);
            lblProfilePath.Text = profileName;
            UserProfileProcessor = new UserProfileProcessor(pathToUserProfile);

            UpdateListBox();
        }

        private void UpdateListBox()
        {
            listBoxSavedFormations.DataSource = null;
            listBoxSavedFormations.DataSource = UserProfileProcessor.FormationSegments;
            listBoxSavedFormations.DisplayMember = "FormationName";
            listBoxSavedFormations.Update();
        }

        private void listBoxSavedFormations_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxSavedFormations.SelectedItems.Count > 1)
            {
                return;
            }

            if (listBoxSavedFormations.SelectedItem == null)
            {
                MessageBox.Show("Please select a formation to edit.");
                return;
            }

            var editor = new frmFormationEditor(PathToUserProfile, UserProfileProcessor, (FormationSegment)listBoxSavedFormations.SelectedItem);
            editor.Show();
            this.Enabled = false;

            editor.FormClosed += (s, args) =>
            {
                this.Close();
            };
        }

        private void btnAddSegment_Click(object sender, EventArgs e)
        {
            if (UserProfileProcessor.FormationSegments.Count > 9)
            {
                MessageBox.Show("You cannot have more than 10 formations in your profile.");
                return;
            }

            var blob = Clipboard.GetText();
            var importedFormations = Helper.DecypherImportedBlob(blob);

            if (importedFormations == null || importedFormations.Length < 1)
            { 
                MessageBox.Show("Could not locate any formations in your clipboard. Please make sure you have copied a valid formation and try again.");
                return;
            }

            if (UserProfileProcessor.FormationSegments.Count + importedFormations.Length > 10)
            { 
                MessageBox.Show($"Importing these formations would exceed the maximum of 10 formations per profile. Please make sure there is enough slots to import {importedFormations.Length} new formations and try again.");
            }

            UserProfileProcessor.FormationSegments.AddRange(importedFormations);
            UpdateListBox();

            UserProfileProcessor.CleanUpOrder();
            UserProfileProcessor.SaveToUserProfile();

            MessageBox.Show("Formation saved!");
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedFormations = new List<FormationSegment>();

            foreach (FormationSegment item in listBoxSavedFormations.SelectedItems)
            {
                selectedFormations.Add(item);
            }

            if (selectedFormations == null || selectedFormations.Count < 2)
            {
                return;
            }

            if (UserProfileProcessor.FormationSegments.Count - selectedFormations.Count < 1)
            {
                MessageBox.Show("You must have at least one formation in your profile.");
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete {selectedFormations.Count} formation(s)?", "Delete Fromations", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (FormationSegment item in selectedFormations)
                {
                    UserProfileProcessor.FormationSegments.Remove(item);
                }

                UpdateListBox();

                UserProfileProcessor.CleanUpOrder();
                UserProfileProcessor.SaveToUserProfile();

                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (listBoxSavedFormations.SelectedItems.Count > 9)
            { 
                MessageBox.Show("You cannot export more than 9 formations at once.");
                return;
            }

            if (listBoxSavedFormations.SelectedItems.Count < 1)
            {
                MessageBox.Show("You must select at least one formation to export.");
                return;
            }

            var selectedFormations = new List<FormationSegment>();
            foreach (FormationSegment formation in listBoxSavedFormations.SelectedItems)
            {
                selectedFormations.Add(formation);
            }

            Clipboard.SetText(Helper.GenerateExportBlobs(selectedFormations));

            MessageBox.Show("Formation copied to clipboard!");
        }
    }
}
