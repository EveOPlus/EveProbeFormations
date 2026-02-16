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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private string _currentFieldsInitialValue = string.Empty;

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
                var textBoxForAzimuth = $"txtAzimuth{formNameCounter}";
                var textBoxForElevation = $"txtElevation{formNameCounter}";
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
                TextBox txtAzimuth = (TextBox)(Controls.Find(textBoxForAzimuth, true)[0]);
                TextBox txtElevation = (TextBox)(Controls.Find(textBoxForElevation, true)[0]);
                TextBox txtNormalVect = (TextBox)(Controls.Find(textBoxForNormalVect, true)[0]);

                if (Helper.RunningInUnlockedMode)
                {
                    txtX.ReadOnly = false;
                    txtY.ReadOnly = false;
                    txtZ.ReadOnly = false;
                    txtSize.ReadOnly = false;
                    txtDistance.ReadOnly = false;
                    txtAzimuth.ReadOnly = false;
                    txtElevation.ReadOnly = false;
                }

                ProbeValueChanged += () => UpdateProbeCalculations(probe, txtDistance, txtAzimuth, txtElevation, txtNormalVect);

                txtX.GotFocus += (s, e) => { _currentFieldsInitialValue = txtX.Text; };
                txtX.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtX.Text, out double newX))
                    {
                        probe.X = newX;
                    }

                    ProbeValueChanged();
                };

                txtY.GotFocus += (s, e) => { _currentFieldsInitialValue = txtY.Text; };
                txtY.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtY.Text, out double newY))
                    {
                        probe.Y = newY;
                    }

                    ProbeValueChanged();
                };

                txtZ.GotFocus += (s, e) => { _currentFieldsInitialValue = txtZ.Text; };
                txtZ.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtZ.Text, out double newZ))
                    {
                        probe.Z = newZ;
                    }

                    ProbeValueChanged();
                };

                txtSize.GotFocus += (s, e) => { _currentFieldsInitialValue = txtSize.Text; };
                txtSize.LostFocus += (s, e) =>
                {
                    if (double.TryParse(txtSize.Text, out double newSize))
                    {
                        probe.DiameterAu = newSize;
                    }
                    else
                    {
                        probe.DiameterAu = 0;
                    }

                    ProbeValueChanged();
                };

                Action<TextBox> reverseCal = (s) =>
                {
                    if (s.Text == _currentFieldsInitialValue)
                    {
                        return;
                    }
                    
                    if (double.TryParse(txtDistance.Text, out double newDistance) &&
                        double.TryParse(txtAzimuth.Text, out double newAzimuth) &&
                        double.TryParse(txtElevation.Text, out double newElevation))
                    {
                        newDistance = newDistance * 1000;

                        // The average center point changes as an average so loop the calculation a few times to bring it closer to the expected result.
                        for (int i = 0; i < 1; i++)
                        {
                            ReverseCalculateVector(probe, newDistance, newAzimuth, newElevation);

                            txtX.Text = probe.X.ToString();
                            txtY.Text = probe.Y.ToString();
                            txtZ.Text = probe.Z.ToString();

                            ProbeValueChanged();
                        }
                    }
                };

                txtDistance.LostFocus += (s, e) => { reverseCal((TextBox)s); };
                txtAzimuth.LostFocus += (s, e) => { reverseCal((TextBox)s); };
                txtElevation.LostFocus += (s, e) => { reverseCal((TextBox)s); };
                txtDistance.GotFocus += (s, e) => { _currentFieldsInitialValue = ((TextBox)s).Text; };
                txtAzimuth.GotFocus += (s, e) => { _currentFieldsInitialValue = ((TextBox)s).Text; };
                txtElevation.GotFocus += (s, e) => { _currentFieldsInitialValue = ((TextBox)s).Text; };
            }

            ProbeValueChanged();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save() 
        {
            SelectedFormation.Probes.RemoveAll(p => !p.IsValid); // Remove empty probes from the the list.
            if (SelectedFormation.Probes.Count < 2)
            {
                MessageBox.Show("A formation must have at least two probes.");
                this.Close();
            }

            Processor.CleanUpOrder();

            Processor.SaveToUserProfile();
            MessageBox.Show("Formation saved!");
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Helper.GenerateExportBlobs(new List<FormationSegment> { SelectedFormation }));

            MessageBox.Show("Formation copied to clipboard!");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var blob = Clipboard.GetText();
            var importedFormations = Helper.DecypherImportedBlob(blob);

            if (importedFormations == null || importedFormations.Length < 1)
            {
                return;
            }

            var importedFormation = importedFormations[0];

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
            Save();
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

            if (validProbes.Count < 2)
            {
                _averageX = 0;
                _averageY = 0;
                _averageZ = 0;

                txtXAvg.Text = "0";
                txtYAvg.Text = "0";
                txtZAvg.Text = "0";
                return;
            }

            var sumOfX = validProbes.Sum(p => p.X);
            var averageX = sumOfX != 0 ? sumOfX / validProbes.Count : 0;

            var sumOfY = validProbes.Sum(p => p.Y);
            var averageY = sumOfY != 0 ? sumOfY / validProbes.Count : 0;

            var sumOfZ = validProbes.Sum(p => p.Z);
            var averageZ = sumOfZ != 0 ? sumOfZ / validProbes.Count : 0;

            txtXAvg.Text = averageX.ToString("0");
            txtYAvg.Text = averageY.ToString("0");
            txtZAvg.Text = averageZ.ToString("0");

            _averageX = Math.Round(averageX, 0);
            _averageY = Math.Round(averageY, 0);
            _averageZ = Math.Round(averageZ, 0);
        }

        private void UpdateProbeCalculations(Probe probe, Control txtDist, Control txtAzimuth, Control txtElevation, Control txtNormalVect)
        {
            UpdateAverage();

            var normX = probe.X - _averageX;
            var normY = probe.Y - _averageY;
            var normZ = probe.Z - _averageZ;

            txtNormalVect.Text = $"{(normX == 0 ? 0 : normX) / 1000:0}, {(normY == 0 ? 0 : normY / 1000):0}, {(normZ == 0 ? 0 : normZ / 1000):0}";

            var magnitude = Math.Sqrt(Math.Pow(normX, 2) + Math.Pow(normY, 2) + Math.Pow(normZ, 2));

            txtDist.Text = (magnitude == 0 ? 0 : magnitude / 1000).ToString("0");

            // This gives you 0 at the horizon, 90 at the sky, -90 at the ground
            // Y is elevation (vertical axis)
            var elevation = Math.Asin(normY / magnitude) * (180 / Math.PI);
            txtElevation.Text = elevation.ToString("0.#");

            var azimuth = Math.Atan2(normX, normZ) * (180 / Math.PI);

            txtAzimuth.Text = azimuth.ToString("0.#");
        }

        private void ReverseCalculateVector(Probe probe, double magnitude, double azimuth, double elevation)
        {
            double azimuthRad = azimuth * (Math.PI / 180.0);
            double elevationRad = elevation * (Math.PI / 180.0);

            double x = magnitude * Math.Cos(elevationRad) * Math.Sin(azimuthRad);
            double y = magnitude * Math.Sin(elevationRad);
            double z = magnitude * Math.Cos(elevationRad) * Math.Cos(azimuthRad);

            // X = Right/Left, Y = Up/Down, Z = Forward/Back
            probe.X = Math.Round(x + _averageX, 0);
            probe.Y = Math.Round(y + _averageY, 0);
            probe.Z = Math.Round(z + _averageZ, 0);
        }

        private void btnReorigin_Click(object sender, EventArgs e)
        {
            var validProbes = SelectedFormation.Probes.FindAll(p => p.IsValid);
            foreach (var probe in validProbes)
            {
                probe.X = Math.Round(probe.X - _averageX, 0);
                probe.Y = Math.Round(probe.Y - _averageY, 0);
                probe.Z = Math.Round(probe.Z - _averageZ, 0);
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
