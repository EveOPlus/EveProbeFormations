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

        public const string SettingsFileName = "settings.json";
        public static Settings Settings = new Settings();

        public static bool TryFindUserAlias(string userId, out string alias)
        {
            alias = string.Empty;
            var matchingAlias = Settings.Aliases.FirstOrDefault(x => x.UserId == userId)?.Alias;

            alias = matchingAlias ?? string.Empty;
            return !string.IsNullOrEmpty(alias);
        }

        public static bool TryFindUserIdInFileName(string fileName, out string userId)
        {
            userId = string.Empty;
            try 
            {
                var prefix = "core_user_";
                var index = fileName.IndexOf(prefix);
                if (index < 0)
                {
                    return false;
                }

                var ss = fileName.Substring(index + prefix.Length, fileName.Length - prefix.Length - index);
                var firstNumbers = new string(ss.TakeWhile(char.IsNumber).ToArray());
                userId = firstNumbers;

                return true;
            }
            catch 
            {
                return false;
            } 
        }

        public static void LoadSettings() 
        {
            try 
            {
                var settingContent = File.ReadAllText(SettingsFileName);
                var newSettings = JsonConvert.DeserializeObject<Settings>(settingContent);

                if (newSettings != null)
                {
                    Settings = newSettings;
                }
            }
            catch 
            {
                SaveSettings();
            }
        }

        public static void SaveSettings()
        {
            File.WriteAllText(SettingsFileName, JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

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

        public static List<UserDatFound> GetUserDatFiles(string settingsFolderPath)
        {
            var result = new List<UserDatFound>();
            if (Directory.Exists(settingsFolderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(settingsFolderPath);
                FileInfo[] files = directoryInfo.GetFiles("core_user_*.dat");
                foreach (FileInfo file in files)
                {
                    var newFoundDat = new UserDatFound();
                    newFoundDat.FilePath = file.FullName;
                    newFoundDat.LastModified = file.LastWriteTime;

                    if (Helper.TryFindUserIdInFileName(file.FullName, out string userId)) 
                    {
                        newFoundDat.UserId = userId;
                    }

                    if (newFoundDat.UserId != null && Helper.TryFindUserAlias(newFoundDat.UserId, out string alias))
                    {
                        newFoundDat.UserAlias = alias;
                    }

                    if (newFoundDat.FileName == "core_user__.dat")
                    {
                        continue;
                    }

                    result.Add(newFoundDat);
                }
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

            return result.OrderByDescending(x => x.LastModified).ToList();
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

        public static string GenerateExportBlobs(IEnumerable<FormationSegment> theSegmentsToExport)
        {
            var cloneJson = JsonConvert.SerializeObject(theSegmentsToExport, Formatting.Indented);
            var clone = JsonConvert.DeserializeObject<List<FormationSegment>>(cloneJson);

            foreach (var segment in clone)
            {
                segment?.Probes.RemoveAll(p => !p.IsValid);
            }

            var json = JsonConvert.SerializeObject(clone, Formatting.Indented);

            var cipherText = CryptoHelper.Encrypt(json);

            var result = new StringBuilder();
            var formationNames = string.Join(", ", theSegmentsToExport.Select(s => s.FormationName));
            var header = $"=== Eve Probe Fromations ({formationNames}) ===";
            result.AppendLine(header);

            foreach (var line in cipherText.SplitLines(header.Length))
            {
                result.AppendLine(line);
            }

            result.AppendLine(new string(header.Select(x => '=').ToArray()));

            return result.ToString();
        }

        public static FormationSegment[]? DecypherImportedBlob(string exportedCipherBlob)
        {
            exportedCipherBlob = exportedCipherBlob.Trim();
            string[] lines = exportedCipherBlob.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (!lines[0].StartsWith("=== Eve Probe Fromation"))
                throw new Exception("Not a valid formatted probe formation.");

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

            if (json.Trim().StartsWith("["))
            { 
                return JsonConvert.DeserializeObject<FormationSegment[]>(json);
            }
            else
            {
                var singleResult = JsonConvert.DeserializeObject<FormationSegment>(json);
                if (singleResult != null)
                {
                    MessageBox.Show("Detected an old format which is no longer supported.");
                }
            }

            return null;
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

        public static DateTime? GetApproximateInternetTime()
        {
            try
            {
                var url = "https://esi.evetech.net/status";
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).GetAwaiter().GetResult();
                    if (response.Headers.Date.HasValue)
                    { 
                        return response.Headers.Date.Value.DateTime;
                    }

                    var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
                    DateTime dateTime = jsonObj.start_time;
                    return dateTime;
                }
            }
            catch { }

            try
            {
                var url = "https://developers.eveonline.com/";
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).GetAwaiter().GetResult();
                    DateTimeOffset responseDate = response.Headers.Date.Value;
                    return responseDate.DateTime;
                }
            }
            catch { }

            return null;
        }

        public static void UpdateUserAlias(string userId, string newAlias)
        {
            var existingAliasSetting = Settings.Aliases.FirstOrDefault(x => x.UserId == userId);
            if (existingAliasSetting != null && newAlias != existingAliasSetting.Alias)
            {
                existingAliasSetting.Alias = newAlias;
            }
            else 
            {
                Settings.Aliases.Add(new UserAlias
                { 
                    UserId = userId,
                    Alias = newAlias
                });
            }

            SaveSettings();
        }
    }
}
