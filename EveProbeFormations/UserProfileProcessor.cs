using System;
using System.Collections.Generic;
using System.Text;

namespace EveProbeFormations
{
    public class UserProfileProcessor
    {
        public string PathToUserProfile { get; set; }

        /// <summary>
        /// All the bytes we see before finding the segment we want to edit.
        /// </summary>
        public List<byte> PreSegmentBytes { get; set; } = new List<byte>();

        /// <summary>
        /// All the bytes after the segment we want to edit.
        /// </summary>
        public List<byte> PostSegmentBytes { get; set; } = new List<byte>();

        public List<FormationSegment> FormationSegments { get; set; } = new List<FormationSegment>();

        public UserProfileProcessor(string pathToUserProfile)
        {
            PathToUserProfile = pathToUserProfile;
            ParseFile();
        }

        private int Position = 0; // how many bytes we have read.

        private byte ReadByte(BinaryReader reader)
        {
            byte b = reader.ReadByte();
            Position++;
            return b;
        }

        private sbyte ReadSByte(BinaryReader reader)
        {
            sbyte b = reader.ReadSByte();
            Position++;
            return b;
        }

        private byte[] ReadBytes(BinaryReader reader, int count)
        {
            byte[] bytes = reader.ReadBytes(count);
            Position += count;
            return bytes;
        }

        private string ReadChars(BinaryReader reader, int count)
        {
            char[] chars = reader.ReadChars(count);
            Position += count;
            return new string(chars);
        }

        private void ParseFile()
        {
            using (FileStream fs = new FileStream(PathToUserProfile, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                SeekToTheProbeFormationSegment(reader);

                bool isMoreFormations = true;

                while (isMoreFormations)
                {
                    var formation = new FormationSegment();
                    FormationSegments.Add(formation);

                    formation.SegmentPrefixDelimiterPosition = Position - 2; // Minus two because we likely read a 0x2c and 0x2e to find the start of this segment.

                    var labelLength = ReadSByte(reader);
                    formation.FormationName = ReadChars(reader, labelLength);

                    if (ReadByte(reader) != 0x15) // this seems to be a static value after the label for probe formations.
                    {
                        continue; // This wasn't a probe formation so skip it.
                    }

                    sbyte totalProbesInFormation = ReadSByte(reader);

                    formation.Probes = ReadAllProbes(reader, totalProbesInFormation);

                    var finalByte1 = ReadByte(reader); // Should be 0x06 or the identifier. Not totally sure why.
                    var finalByte2 = ReadByte(reader);

                    if (finalByte2 != 0x2c && finalByte1 == 0x06)
                    {
                        formation.Has0x06AtFinalBytes = true;
                        formation.SegmentIdentityByte = finalByte2;
                    }
                    else
                    {
                        formation.Has0x06AtFinalBytes = false;
                        formation.SegmentIdentityByte = finalByte1;
                    }

                    var endOfSegmentDelimiterByte = formation.Has0x06AtFinalBytes ? ReadByte(reader) : finalByte2; // Should be 0x2c but only if there are more to process.
                    if (endOfSegmentDelimiterByte == 0x2c)
                    {
                        ReadByte(reader); // == 0x2e
                        // there is at least one more formation to process.
                        isMoreFormations = true;
                    }
                    else
                    {
                        // there are no more formations to process. We have reached the end of this segment.
                        isMoreFormations = false;
                        PostSegmentBytes.Add(endOfSegmentDelimiterByte);
                    }
                }

                ReadToEnd(reader);
            }
        }

        public List<Probe> ReadAllProbes(BinaryReader reader, sbyte totalProbesInFormation)
        {
            var probes = new List<Probe>();
            for (int i = 0; i < totalProbesInFormation; i++)
            {
                ReadByte(reader); // throw away the next delimiter
                probes.Add(ReadSingleProbe(reader));
            }

            return probes;
        }

        public Probe ReadSingleProbe(BinaryReader reader)
        {
            var probe = new Probe();
            if (ReadByte(reader) != 0x14) throw new Exception("Unexpected value"); // throw away the 0x14 (static value)
            if (ReadByte(reader) != 0x03) throw new Exception("Unexpected value"); // throw away the 0x03 (static value)

            if (ReadByte(reader) == 0x0a) // 0x0A seems to mean the next 8 bytes are the value. 0x0B seems to mean 0 and no need to read more bytes for the next value.
            {
                probe.X = reader.ReadDouble();
            }

            if (ReadByte(reader) == 0x0a)
            {
                probe.Y = reader.ReadDouble();
            }

            if (ReadByte(reader) == 0x0a)
            {
                probe.Z = reader.ReadDouble();
            }

            if (ReadByte(reader) == 0x0a)
            {
                probe.DimeterRaw = reader.ReadDouble();
            }

            return probe;
        }

        public void SeekToTheProbeFormationSegment(BinaryReader reader)
        {
            try
            {
                while (true)
                {
                    var firstByte = ReadByte(reader);

                    if (firstByte == 0x2c)
                    {
                        var secondByte = ReadByte(reader);

                        if (secondByte == 0x2e)
                        {
                            break;
                        }

                        PreSegmentBytes.Add(firstByte);
                        PreSegmentBytes.Add(secondByte);
                    }
                    else
                    {
                        PreSegmentBytes.Add(firstByte);
                    }
                }
            }
            catch (EndOfStreamException eos)
            {
                throw new Exception("Unable to locate any saved probe formations.");
            }
        }

        public void ReadToEnd(BinaryReader reader)
        {
            try
            {
                while (true)
                {
                    PostSegmentBytes.Add(ReadByte(reader));
                }
            }
            catch (EndOfStreamException eos)
            {
                // we have reached the end of the file which is expected.
            }
        }

        public void BackupUserProfile()
        {
            if (PathToUserProfile == null)
            {
                return;
            }

            string backupFolder = Path.Combine(Path.GetDirectoryName(PathToUserProfile) ?? throw new Exception(), "backups");

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
            var backupFileName = Path.GetFileNameWithoutExtension(PathToUserProfile) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(PathToUserProfile);

            var backupPath = Path.Combine(backupFolder, backupFileName);
            File.Copy(PathToUserProfile, backupPath);
        }

        public List<byte> GenerateFormationSectionBytes()
        {
            var bytes = new List<byte>();

            foreach (var formation in FormationSegments)
            {
                bytes.Add(0x2c); // add the 0x2c delimiter before each segment.
                bytes.Add(0x2e); // add the 0x2e marker before each segment.
                bytes.AddRange(formation.ToBytes());
            }

            return bytes;
        }

        public byte[] GenerateNewUserProfileBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(PreSegmentBytes);
            bytes.AddRange(GenerateFormationSectionBytes());
            bytes.AddRange(PostSegmentBytes);

            return bytes.ToArray();
        }

        public void CleanUpOrder()
        {
            int newIdentifier = 10;
            foreach (var formation in FormationSegments)
            {
                formation.SegmentIdentityByte = (byte)newIdentifier;

                formation.Has0x06AtFinalBytes = newIdentifier > 2 || FormationSegments.Count < 3;
                newIdentifier++;
            }

            PreSegmentBytes.RemoveAt(PreSegmentBytes.Count - 1);
            PreSegmentBytes.Add(Convert.ToByte(FormationSegments.Count)); // The final byte before the first segment seems to indicate how many formations there are. We need to update this if we add a formation.
        }

        public void SaveToUserProfile()
        {
            BackupUserProfile();
            var newBytes = GenerateNewUserProfileBytes();

            File.WriteAllBytes(PathToUserProfile, newBytes);
        }
    }
}
