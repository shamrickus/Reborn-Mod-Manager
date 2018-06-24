using System.IO;
using System.Linq;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace DotaInstaller.DotaModel
{
    public static class DotaLocator
    {
        public static bool Checked;
        public static string LocateSteam()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
            {
                return key?.GetValue("InstallPath").ToString();
            }
        }

        public const string APP_ID = "570";

        public static string LocateDota(string pPath)
        {
            var path = Path.Combine(pPath, "steamapps", $"appmanifest_{APP_ID}.acf");
            if (File.Exists(path))
            {
                string line;
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    var reader = new StreamReader(file);
                    while((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("installdir"))
                        {
                            var name = line.Split('\t').Last().Trim('\"');
                            return Path.Combine(pPath, "steamapps", "common", name);
                        }
                    }
                }
            }

            return null;
        }

        public static string ReadExternalGameFolder(string pPath)
        {
            var path = Path.Combine(pPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(path))
            {
                string line;
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    var reader = new StreamReader(file);
                    while ((line = reader.ReadLine()) != null)
                    {
                    }
                }
            }

            return null;
        }
    }
}
