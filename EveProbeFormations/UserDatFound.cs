using System;
using System.Collections.Generic;
using System.Text;

namespace EveProbeFormations
{
    public class UserDatFound
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName => Path.GetFileName(FilePath);
        public string Profile => GetProfile();
        public string? UserId { get; set; }
        public string? UserAlias { get; set; }
        public DateTime? LastModified { get; set; }
        public string Display => GetDisplay();

        public string GetDisplay() 
        {
            var aliasPart = UserAlias == null ? null : $"Alias: {UserAlias}    ";
            var lastModPart = string.Empty;
            if (LastModified != null)
            {
                lastModPart += "    Last Modified: ";

                TimeSpan timeSinceModified = DateTime.Now - LastModified.Value;

                var totalMinutes  = (int)timeSinceModified.TotalMinutes;

                if (totalMinutes < 1)
                {
                    lastModPart += "just now";
                }
                else
                {
                    if (timeSinceModified.TotalHours > 24)
                    {
                        lastModPart += " > 1 day ago";
                    }
                    else
                    {
                        var totalHours = (int)timeSinceModified.TotalHours;
                        if (totalHours > 0)
                        {
                            lastModPart += $" {totalHours} hour";

                            if (totalHours > 1)
                            {
                                lastModPart += "s";
                            }
                        }

                        var minutes = (int)timeSinceModified.Minutes;
                        lastModPart += $" {minutes} minutes ago";
                    }
                }
            }

            return $"{aliasPart}Profile: {Profile}    File: {FileName}{lastModPart}";
        }

        public string GetProfile() 
        {
            try 
            {
                var folders = FilePath.Split('\\').Reverse();
                foreach (var folder in folders)
                {
                    if (folder.StartsWith("settings_"))
                    {
                        var secondSplit = folder.Split('_');
                        return secondSplit[1];
                    }
                }
            }
            catch { }

            return "Unknown";
        }
    }
}
