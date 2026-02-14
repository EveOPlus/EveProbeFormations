using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveProbeFormations
{
    public class Probe
    {
        public double X { get; set; } = 0d;
        public double Y { get; set; } = 0d;
        public double Z { get; set; } = 0d;
        public double DimeterRaw { get; set; } = 0d;
        public double DiameterAu 
        {
            get
            {
                return DimeterRaw > 0 ? DimeterRaw / 149597870700d : 0d; // convert from meters to AU.
            }
            set
            {
                DimeterRaw = value * 149597870700d; // convert from AU to meters.
            }
        }

        [JsonIgnore]
        public bool IsValid => CheckIsValid();

        private bool CheckIsValid() 
        {
            if (double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z) || double.IsNaN(DimeterRaw) || DimeterRaw < 1d)
            {
                return false;
            }

            if (DimeterRaw <= 1d)
            {
                return false;
            }

            return true;
        }
    }
}
