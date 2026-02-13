using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EveProbeFormations
{
    public partial class frmFormationEditor : Form
    {
        public string PathToProfile { get; }
        public UserProfileProcessor Processor { get; }

        public delegate void ProbeValueChangedDelegate();
        public event ProbeValueChangedDelegate ProbeValueChanged;

        private double _averageX;
        private double _averageY;
        private double _averageZ;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FormationSegment SelectedFormation { get; set; }

        public frmFormationEditor(string pathToProfile, UserProfileProcessor processor, FormationSegment selectedFormation)
        {
            InitializeComponent();
            PathToProfile = pathToProfile;
            Processor = processor;
            SelectedFormation = selectedFormation;

            LoadSelectedFormationToForm();
        }

        private void PadEmptyProbes()
        {
            var emptyProbes = 8 - SelectedFormation.Probes.Count;
            for (int i = 0; i < emptyProbes; i++)
            {
                SelectedFormation.Probes.Add(new Probe());
            }
        }

        private void LoadSelectedFormationToForm()
        {
            PadEmptyProbes();
            txtFormationName.Text = SelectedFormation.FormationName;
            txtFormationName.LostFocus += (s, e) =>
            {
                SelectedFormation.FormationName = txtFormationName.Text;
            };

            int formNameCounter = 1;
            foreach (var probe in SelectedFormation.Probes)
            {
                var textBoxForX = $"txtX{formNameCounter}";
                var textBoxForY = $"txtY{formNameCounter}";
                var textBoxForZ = $"txtZ{formNameCounter}";
                var textBoxForSize = $"txtSize{formNameCounter}";
                var textBoxForDistance = $"txtDistance{formNameCounter}";
                var textBoxForLat = $"txtLat{formNameCounter}";
                var textBoxForLong = $"txtLong{formNameCounter}";
                var textBoxForNormalVect = $"txtNormalVect{formNameCounter}";
                formNameCounter++;

                TextBox txtX = (TextBox)(Controls.Find(textBoxForX, true)[0]);
                txtX.Text = probe.X.ToString();

                TextBox txtY = (TextBox)(Controls.Find(textBoxForY, true)[0]);
                txtY.Text = probe.Y.ToString();

                TextBox txtZ = (TextBox)(Controls.Find(textBoxForZ, true)[0]);
                txtZ.Text = probe.Z.ToString();

                TextBox txtSize = (TextBox)(Controls.Find(textBoxForSize, true)[0]);
                txtSize.Text = probe.DiameterAu.ToString();

                TextBox txtDistance = (TextBox)(Controls.Find(textBoxForDistance, true)[0]);
                TextBox txtLat = (TextBox)(Controls.Find(textBoxForLat, true)[0]);
                TextBox txtLong = (TextBox)(Controls.Find(textBoxForLong, true)[0]);
                TextBox txtNormalVect = (TextBox)(Controls.Find(textBoxForNormalVect, true)[0]);

                if (Helper.RunningInUnlockedMode)
                {
                    txtX.ReadOnly = false;
                    txtY.ReadOnly = false;
                    txtZ.ReadOnly = false;
                    txtSize.ReadOnly = false;
                }

                ProbeValueChanged += () => UpdateProbeCalculations(probe, txtDistance, txtLat, txtLong, txtNormalVect);

                txtX.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtX.Text, out double newX))
                    {
                        probe.X = newX;
                    }

                    ProbeValueChanged();
                };

                txtY.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtY.Text, out double newY))
                    {
                        probe.Y = newY;
                    }

                    ProbeValueChanged();
                };

                txtZ.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtZ.Text, out double newZ))
                    {
                        probe.Z = newZ;
                    }

                    ProbeValueChanged();
                };

                txtSize.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtSize.Text, out double newSize))
                    {
                        probe.DiameterAu = newSize;
                    }

                    ProbeValueChanged();
                };
            }

            ProbeValueChanged();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SelectedFormation.Probes.RemoveAll(p => !p.IsValid); // Remove empty probes from the the list.
            if (SelectedFormation.Probes.Count == 0)
            {
                MessageBox.Show("A formation must have at least one probe.");
                this.Close();
            }

            Processor.CleanUpOrder();

            Processor.SaveToUserProfile();
            MessageBox.Show("Formation saved!");
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Helper.GenerateExportBlob(SelectedFormation));

            MessageBox.Show("Formation copied to clipboard!");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var blob = Clipboard.GetText();
            var importedFormation = Helper.ImportBlobCipher(blob);

            if (importedFormation == null)
            {
                MessageBox.Show("Clipboard does not contain a valid formation.");
                return;
            }

            for (int i = 0; i < Processor.FormationSegments.Count; i++)
            {
                if (Processor.FormationSegments[i] == SelectedFormation)
                {
                    Processor.FormationSegments[i] = importedFormation;
                    SelectedFormation = importedFormation;
                }
            }

            LoadSelectedFormationToForm();
        }

        private void btnOrderToTop_Click(object sender, EventArgs e)
        {
            Processor.FormationSegments.Remove(SelectedFormation);
            Processor.FormationSegments.Insert(0, SelectedFormation);

            MessageBox.Show("Formation will be moved to the top when you save.");
        }

        private void UpdateAverage()
        {
            var validProbes = SelectedFormation.Probes.FindAll(p => p.IsValid);

            var sumOfX = validProbes.Sum(p => p.X);
            var averageX = sumOfX != 0 ? sumOfX / validProbes.Count : 0;

            var sumOfY = validProbes.Sum(p => p.Y);
            var averageY = sumOfY != 0 ? sumOfY / validProbes.Count : 0;

            var sumOfZ = validProbes.Sum(p => p.Z);
            var averageZ = sumOfZ != 0 ? sumOfZ / validProbes.Count : 0;

            txtXAvg.Text = averageX.ToString("0");
            txtYAvg.Text = averageY.ToString("0");
            txtZAvg.Text = averageZ.ToString("0");

            _averageX = averageX;
            _averageY = averageY;
            _averageZ = averageZ;
        }

        private void UpdateProbeCalculations(Probe probe, Control txtDist, Control txtLat, Control txtLong, Control txtNormalVect)
        {
            if (!probe.IsValid)
            {
                return;
            }

            UpdateAverage();

            // Calculate the distance from the average point and update the distance text box for the probe.
            var centerX = double.TryParse(txtXAvg.Text, out double cx) ? cx : 0;
            var centerY = double.TryParse(txtYAvg.Text, out double cy) ? cy : 0;
            var centerZ = double.TryParse(txtZAvg.Text, out double cz) ? cz : 0;

            // Normalize the vector by moving the center to origin.
            var normX = probe.X - centerX;
            var normY = probe.Y - centerY;
            var normZ = probe.Z - centerZ;

            txtNormalVect.Text = $"{(normX == 0 ? 0 : normX) / 1000:0}, {(normY == 0 ? 0 : normX / 1000):0}, {(normX == 0 ? 0 : normZ / 1000):0}";

            var magnitude = Math.Sqrt(Math.Pow(normX, 2) + Math.Pow(normY, 2) + Math.Pow(normZ, 2));

            txtDist.Text = (magnitude == 0 ? 0 : magnitude / 1000).ToString("0");

            // Calculate the latitude from the normalized 3d vector and update the Latitude text box for the probe.
            var latitude = Math.Asin(normZ / magnitude) * (180 / Math.PI);
            txtLat.Text = latitude.ToString("0.#");

            // Calculate the longitude 3d vector and update the Longitude text box for the probe.
            var longtitude = Math.Atan2(normY, normX) * (180 / Math.PI);
            txtLong.Text = longtitude.ToString("0.#");
        }

        private void btnReorigin_Click(object sender, EventArgs e)
        {
            var validProbes = SelectedFormation.Probes.FindAll(p => p.IsValid);
            foreach (var probe in validProbes)
            {
                probe.X = probe.X - _averageX;
                probe.Y = probe.Y - _averageY;
                probe.Z = probe.Z - _averageZ;
            }

            LoadSelectedFormationToForm();
        }

        private void frmFormationEditor_Load(object sender, EventArgs e)
        {
            if (Helper.RunningInUnlockedMode)
            {
                groupBoxVectors.Text = "Editable";
                btnReorigin.Enabled = true;
                btnReorigin.Visible = true;
            }
        }
    }
}
