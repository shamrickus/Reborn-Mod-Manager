using System.Collections.Generic;
using System.IO;

namespace DotaInstaller
{
    class Dota2Tome
    {
        public static string GAMEINFO = @"\game\dota\gameinfo.gi";

        public static void ReadFile(string SteamLocation, string pModName)
        {
            new BGWorker(() =>
            {
                string line;
                List<string> fileData = new List<string>();
                using (var file = File.Open($"{SteamLocation}{GAMEINFO}", FileMode.Open, FileAccess.Read))
                {

                    var reader = new StreamReader(file);
                    var insertString = "\t\t\tGame_LowViolence\tdota_lv";
                    var insertValue = $"\t\t\tGame\t\t\t\t{pModName}";
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(line != insertValue)
                            fileData.Add(line);
                        if (line == insertString)
                            fileData.Add(insertValue);

                    }
                }

                using (var tw = new StreamWriter(SteamLocation + GAMEINFO))
                {
                    foreach(var lineData in fileData)
                        tw.WriteLine(lineData);
                }
                return null;
            });
        }

        public static void CreateAndCopy(string SteamLocation, string pModName)
        {
            Directory.CreateDirectory($@"{SteamLocation}\game\{pModName}");
            File.Copy($@"{Directory.GetCurrentDirectory()}\{VpkCompiler.VPK_COMP}", $@"{SteamLocation}\game\{pModName}\{VpkCompiler.VPK_COMP}", true);
        }

        public static bool ConfigExists(string SteamLocation)
        {
            return File.Exists($"{SteamLocation}{GAMEINFO}");
        }
    }
}
