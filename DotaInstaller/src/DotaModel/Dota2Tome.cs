using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DotaInstaller.src.Utilities
{
    internal static class Dota2Tome
    {
        public delegate void OnLocationChanged();
        public static event OnLocationChanged LocationChanged;

        public static string GAMEINFO = @"\game\dota\gameinfo.gi";

        public static void ReadFile(string SteamLocation, string pName)
        {
            new SyncThread(() =>
            {
                string line;
                List<string> fileData = new List<string>();
                using (var file = File.Open(SteamLocation + GAMEINFO, FileMode.Open, FileAccess.Read))
                {
                    var reader = new StreamReader(file);
                    var dotaLv = "\t\t\tGame_LowViolence\tdota_lv";
                    var addonLine = $"\t\t\tGame\t\t\t\t{pName}";
                    var lvFound = false;
                    var dotaFound = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(lvFound && dotaFound)
                            fileData.Add(line);
                        else if (lvFound)
                        {
                            if (line.Contains("dota") || line.Contains("core"))
                            {
                                fileData.Add(line);
                                dotaFound = true;
                            }
                            else if(string.IsNullOrEmpty(line))
                                fileData.Add(line);
                        }
                        else
                            fileData.Add(line);
                        if (line == dotaLv)
                        {
                            fileData.Add(addonLine);
                            lvFound = true;
                        }
                    }
                }

                using (var tw = new StreamWriter(SteamLocation + GAMEINFO))
                {
                    foreach (var lineData in fileData)
                        tw.WriteLine(lineData);
                }
                return null;
            });
        }
        public static bool Install(ModPack.ModPack pContainer)
        {
            VpkCompiler.Clean();
            ReadFile(SteamLocation, pContainer.Name);

            pContainer.Copy();
            VpkCompiler.Run();

            CreateAndCopy(SteamLocation, pContainer.Name);
            return true;
        }

        private static string _steamLocation;
        public static  string SteamLocation
        {
            get
            {
                if (string.IsNullOrEmpty(_steamLocation))
                    SteamLocation = ConfigurationManager.AppSettings[nameof(SteamLocation)];
                return _steamLocation;
            }
            set
            {
                _steamLocation = value;
#if Release
                Config.Set(nameof(SteamLocation), _steamLocation);
#endif
                Error = !ConfigExists();
                LocationChanged?.Invoke();

                if (Error)
                    MessageBox.Show("Required files are not able to be found in that location!");
            }
        }
        public static bool Error { get; set; }

        public static void UpdateLocation()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Where is your Dota 2 folder located?";
                dialog.SelectedPath = SteamLocation;
                var result = dialog.ShowDialog(Application.Current.MainWindow.GetIWin32Window());
                if (result == DialogResult.OK || result == DialogResult.Yes)
                    SteamLocation = dialog.SelectedPath;
            }
        }


        public static void CreateAndCopy(string SteamLocation, string pName)
        {
            Directory.CreateDirectory(Path.Combine(SteamLocation, "game", pName));
            File.Copy(Path.Combine(Directory.GetCurrentDirectory(), VpkCompiler.VPK_COMP), Path.Combine(SteamLocation, "game", pName, VpkCompiler.VPK_COMP), true);
        }

        public static bool ConfigExists()
        {
            return File.Exists($"{SteamLocation}{GAMEINFO}");
        }
    }
}