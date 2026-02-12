using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                MessageBox.Show("You cannot have more than 10 formations in your profile. Hint: Modify one of the existing ones instead.");
                return;
            }

            var json = JsonConvert.SerializeObject(listBoxSavedFormations.SelectedItem);
            var clone = JsonConvert.DeserializeObject<FormationSegment>(json);

            UserProfileProcessor.FormationSegments.Add(clone);
            UpdateListBox();
        }
    }
}
