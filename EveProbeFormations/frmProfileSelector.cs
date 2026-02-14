namespace EveProbeFormations
{
    public partial class frmProfileSelector : Form
    {
        private string pathToEveSettingsFolder = string.Empty;

        public frmProfileSelector()
        {
            InitializeComponent();
        }

        private void frmProfileSelector_Load(object sender, EventArgs e)
        {
            if (!Helper.RunningInUnlockedMode)
            {
                MessageBox.Show("This software is provided \"as is\", without warranties of any kind, disclaiming liability for damages, including negligence, resulting from its use.");
            }

            pathToEveSettingsFolder = Helper.TryToFindPathToLocalEve();
            if (!string.IsNullOrEmpty(pathToEveSettingsFolder))
            {
                RefreshUserDatPaths();
            }
        }

        private void btnRefreshSettingsPath_Click(object sender, EventArgs e)
        {
            RefreshUserDatPaths();
        }

        private void RefreshUserDatPaths()
        {
            listBoxUserDatPaths.Items.Clear();
            var files = Helper.GetUserDatFiles(pathToEveSettingsFolder);

            foreach (var file in files)
            {
                listBoxUserDatPaths.Items.Add(file);
            }
        }

        private void listBoxUserDatPaths_DoubleClick(object sender, EventArgs e)
        {
            var profilePath = listBoxUserDatPaths.SelectedItem?.ToString();
            if (profilePath == null)
            {
                return;
            }

            var newForm = new frmProbeFormationSelector(profilePath);
            newForm.Show();

            newForm.FormClosed += (o, args) => { 
                this.Enabled = true;
                this.BringToFront();
            };
            this.Enabled = false;
        }

        private void btnSettingsFolderPicker_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    pathToEveSettingsFolder = fbd.SelectedPath;
                    RefreshUserDatPaths();
                }
            }
        }
    }
}
