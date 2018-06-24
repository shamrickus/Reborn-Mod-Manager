using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using DotaInstaller.Utilities;

namespace DotaInstaller.DotaModel
{
    internal static class Dota2Tome
    {
        public delegate void OnLocationChanged();
        public static event OnLocationChanged LocationChanged;

        static Dota2Tome()
        {
            SteamLocation = ConfigurationManager.AppSettings[nameof(SteamLocation)];
            if(Error)
                TryFindSteam();
        }

        public static string GAMEINFO = @"\game\dota\gameinfo.gi";

        public static void ReadFile(string pName)
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
            ReadFile(pContainer.Name);

            pContainer.Copy();
            VpkCompiler.Run();

            CreateAndCopy(SteamLocation, pContainer.Name);
            return true;
        }

        private static string _steamLocation;
        public static  string SteamLocation
        {
            get => _steamLocation;
            set
            {
                _steamLocation = value;
#if Release
                Config.Set(nameof(SteamLocation), _steamLocation);
#endif
                Error = !ConfigExists();
                LocationChanged?.Invoke();
            }
        }
        public static bool Error { get; set; }

        public static void UpdateLocation()
        {
            SteamLocation = Dialog.FolderBrowser("Dota 2 location (steam/steamapps/common)", SteamLocation);
        }

        public static void TryFindSteam()
        {
            if (!DotaLocator.Checked)
            {
                DotaLocator.Checked = true;
                var steamPath = DotaLocator.LocateSteam();
                if (steamPath != null)
                {
                    var dotaPath = DotaLocator.LocateDota(steamPath);
                    if (dotaPath != null)
                        SteamLocation = dotaPath;
                }
            }
        }


        public static void CreateAndCopy(string SteamLocation, string pName)
        {
            Directory.CreateDirectory(Path.Combine(SteamLocation, "game", pName));
            var file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), VpkCompiler.VPK_COMP));
            while (file.Exists == false)
            {
                Thread.Sleep(100);
                file.Refresh();
            }
            File.Copy(Path.Combine(Directory.GetCurrentDirectory(), VpkCompiler.VPK_COMP), Path.Combine(SteamLocation, "game", pName, VpkCompiler.VPK_COMP), true);
        }

        public static bool ConfigExists()
        {
            return File.Exists($"{SteamLocation}{GAMEINFO}");
        }
    }
}