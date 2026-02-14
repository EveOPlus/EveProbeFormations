using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace EveProbeFormations
{
    public static class Helper
    {
        public static bool RunningInUnlockedMode = false;

        public static string? TryToFindPathToLocalEve()
        {
            try
            {
                var eveLocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CCP\EVE";
                if (System.IO.Directory.Exists(eveLocalAppDataPath))
                {
                    return eveLocalAppDataPath;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static List<string> GetUserDatFiles(string settingsFolderPath)
        {
            var result = new List<string>();
            if (Directory.Exists(settingsFolderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(settingsFolderPath);
                FileInfo[] files = directoryInfo.GetFiles("core_user_*.dat");
                result.AddRange(files.OrderByDescending(x => x.LastWriteTime).Select(x => x.FullName));
            }

            var subFolders = Directory.GetDirectories(settingsFolderPath);
            foreach (var subFolder in subFolders)
            {
                var thisFolderName = new DirectoryInfo(subFolder).Name.ToLower();
                if (thisFolderName.Contains("eve") && (thisFolderName.Contains("tq") || thisFolderName.Contains("tranq")))
                {
                    result.AddRange(GetUserDatFiles(subFolder));
                }

                if (thisFolderName.StartsWith("settings_"))
                {
                    result.AddRange(GetUserDatFiles(subFolder));
                }
            }

            return result;
        }

        public static bool Like(this double myDouble, double comparisonValue)
        {
            return myDouble > comparisonValue - 1d && myDouble < comparisonValue + 1d;
        }

        public static List<string> SplitLines(this string inputString, int maxLengthPerLine)
        {
            List<string> lines = new List<string>();

            if (inputString.Length == 0 || maxLengthPerLine < 1)
            {
                return lines;
            }

            var lineCount = inputString.Length / maxLengthPerLine;
            var remainder = inputString.Length % maxLengthPerLine;

            for (int i = 0; i < lineCount; i++)
            {
                var charsSkipped = i * maxLengthPerLine;
                var line = inputString.Substring(charsSkipped, maxLengthPerLine);
                lines.Add(line);
            }

            if (remainder > 0)
            { 
                var charsSkipped = inputString.Length - remainder;
                var line = inputString.Substring(charsSkipped, remainder);
                lines.Add(line);
            }

            return lines;
        }

        public static string GenerateExportBlob(FormationSegment theSegmentToExport)
        {
            var cloneJson = JsonConvert.SerializeObject(theSegmentToExport, Formatting.Indented);
            var clone = JsonConvert.DeserializeObject<FormationSegment>(cloneJson);
            clone?.Probes.RemoveAll(p => !p.IsValid);

            var json = JsonConvert.SerializeObject(clone, Formatting.Indented);

            var cipherText = CryptoHelper.Encrypt(json);

            var result = new StringBuilder();
            var header = $"=== Eve Probe Fromation ({theSegmentToExport.FormationName}) ===";
            result.AppendLine(header);

            foreach (var line in cipherText.SplitLines(header.Length))
            {
                result.AppendLine(line);
            }

            result.AppendLine(new string(header.Select(x => '=').ToArray()));

            return result.ToString();
        }

        public static FormationSegment ImportBlobCipher(string exportedCipherBlob)
        {
            exportedCipherBlob = exportedCipherBlob.Trim();
            string[] lines = exportedCipherBlob.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (!lines[0].StartsWith("=== Eve Probe Fromation"))
                throw new Exception("Not a valid formatted prove formation.");

            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.StartsWith("==="))
                {
                    continue;
                }

                sb.Append(line);
            }

            var cipherText = sb.ToString().Trim(); ;

            var json = CryptoHelper.Decrypt(cipherText);

            return JsonConvert.DeserializeObject<FormationSegment>(json);
        }

        public static bool Like(this byte[] arrayA, byte[] arrayB)
        {
            if (arrayA == null || arrayB == null || arrayA.Length != arrayB.Length)
            {
                return false;
            }

            for (int i = 0; i < arrayA.Length; i++)
            {
                if (arrayA[i] != arrayB[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
