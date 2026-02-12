using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EveProbeFormations
{
    public class FormationSegment
    {
        [JsonIgnore]
        public int SegmentPrefixDelimiterPosition { get; set; } // the position of the , immediately before this segment.
        public List<Probe> Probes { get; set; } = new List<Probe>();
        public string FormationName { get; set; } = string.Empty;

        [JsonIgnore]
        public byte ProbeCountByte => Convert.ToByte(Probes.Count);

        [JsonIgnore]
        public byte SegmentPrefixByte { get; set; } = 0x2e;

        [JsonIgnore]
        public byte SegmentIdentityByte { get; set; } // seems to be a count that increments each time a new formation is saved.
       
        [JsonIgnore]
        public byte LabelLengthByte => Convert.ToByte(FormationName.Length);
        
        [JsonIgnore]
        public byte[] LabelBytes => Encoding.ASCII.GetBytes(FormationName);
        
        [JsonIgnore]
        public byte LabelSuffixByte { get; set; } = 0x15;
        
        [JsonIgnore]
        public bool Has0x06AtFinalBytes { get; set; } // Not sure why this is here. It looks it does not set this byte on the first two formations and then all further formations do. Or all formation if theres less than two of them.

        public List<byte> ToBytes()
        {
            var bytes = new List<byte>();

            bytes.Add(LabelLengthByte);
            bytes.AddRange(LabelBytes);
            bytes.Add(0x15);
            bytes.Add(ProbeCountByte);

            foreach (var probe in Probes)
            {
                bytes.Add(0x2c);
                bytes.Add(0x14);
                bytes.Add(0x03);

                if (probe.X.Like(0d))
                {
                    bytes.Add(0x0b);
                }
                else
                {
                    bytes.Add(0x0a);
                    bytes.AddRange(BitConverter.GetBytes(probe.X));
                }

                if (probe.Y.Like(0d))
                {
                    bytes.Add(0x0b);
                }
                else
                {
                    bytes.Add(0x0a);
                    bytes.AddRange(BitConverter.GetBytes(probe.Y));
                }

                if (probe.Z.Like(0d))
                {
                    bytes.Add(0x0b);
                }
                else
                {
                    bytes.Add(0x0a);
                    bytes.AddRange(BitConverter.GetBytes(probe.Z));
                }

                if (probe.DimeterRaw.Like(0d))
                {
                    bytes.Add(0x0b);
                }
                else
                {
                    bytes.Add(0x0a);
                    bytes.AddRange(BitConverter.GetBytes(probe.DimeterRaw));
                }
            }

            if (Has0x06AtFinalBytes)
            { 
                bytes.Add(0x06);
            }

            bytes.Add(SegmentIdentityByte);

            return bytes;
        }
    }
}
