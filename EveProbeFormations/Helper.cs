using System;
using System.Collections.Generic;
using System.Text;

namespace EveProbeFormations
{
    public static class Helper
    {
        public static string? TryToFindPathToDefaultSettings()
        {
            try
            {
                var tranqPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CCP\EVE\c_ccp_eve_tq_tranquility";
                string? path = tranqPath + @"\settings_Default";
                if (!System.IO.Directory.Exists(path))
                {
                    var settingsOptions = Directory.GetDirectories(tranqPath, "settings_*");
                    if (settingsOptions.Any())
                    {
                        path = settingsOptions.FirstOrDefault();
                    }
                }

                return path;
            }
            catch
            {
                return null;
            }
        }

        public static List<string> GetUserDatFiles(string settingsFolderPath)
        {
            if (Directory.Exists(settingsFolderPath))
            {
                return Directory.GetFiles(settingsFolderPath, "core_user_*.dat").OrderBy(x => x).ToList();
            }

            return new List<string>();
        }

        public static bool Like(this double myDouble, double comparisonValue)
        {
            return myDouble > comparisonValue - 1d && myDouble < comparisonValue + 1d;
        }
    }
}
