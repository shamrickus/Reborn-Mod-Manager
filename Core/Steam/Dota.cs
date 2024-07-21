using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Core.Mod;

namespace Core
{
    public class Dota : SteamProduct
    {
        private string GameInfoPath()
        {
            return Path.Join(_location, "game", "dota", "gameinfo.gi");
        }

        private string AddonLine(string pName) => $"\t\t\tGame\t\t\t\t{pName}";

        public string ReadFile(string pName)
        {
            string line;
            List<string> fileData = new List<string>();
            try
            {
                using (var file = File.Open(GameInfoPath(), FileMode.Open, FileAccess.Read))
                {
                    var reader = new StreamReader(file);
                    var dotaLv = "\t\t\tGame_LowViolence\tdota_lv";
                    var lvFound = false;
                    var dotaFound = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (lvFound && dotaFound)
                            fileData.Add(line);
                        else if (lvFound)
                        {
                            if (line.Contains("dota") || line.Contains("core"))
                            {
                                fileData.Add(line);
                                dotaFound = true;
                            }
                            else if (string.IsNullOrEmpty(line))
                                fileData.Add(line);
                            else
                            {
                                Console.WriteLine("Already installed.");
                            }
                        }
                        else
                            fileData.Add(line);
                        if (line == dotaLv)
                        {
                            fileData.Add(AddonLine(pName));
                            lvFound = true;
                        }
                    }
                }
            }
            catch (IOException)
            {
                return $"Mods cannot be installed while Dota is running ({GameInfoPath()} is in use).";
            }

            using (var tw = new StreamWriter(GameInfoPath()))
            {
                foreach (var lineData in fileData)
                    tw.Write(lineData + "\r\n");
            }

            return null;
        }

        public int TrySDKInstalls(Steam pSteam)
        {
            string sdkLocation = null;
            foreach (var sdk in AppIDs.SourceSDK2013())
            {
                sdkLocation = pSteam.LocateGame(sdk);
                if (sdkLocation != null)
                {
                    try
                    {
                        return VpkCompiler.Run(Path.Combine(sdkLocation, "bin"));
                    }
                    catch {}
                }
            }

            return 1;
        }

        private bool removeFile(string pName)
        {
            List<string> fileData = new List<string>();
            string line;
            bool deleted = false;
            using (var file = File.Open(GameInfoPath(), FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(file);
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == AddonLine(pName))
                    {
                        deleted = true;
                        continue;
                    }
                    fileData.Add(line);
                }
            }

            using (var tw = new StreamWriter(GameInfoPath()))
            {
                foreach (var lineData in fileData)
                    tw.Write(lineData + "\r\n");
            }
            return deleted;
        }

        private string runPrepare(ModPack pContainer)
        {
            VpkCompiler.Clean();
            VpkCompiler.Create();

            var result = ReadFile(pContainer.Name);
            if (result != null)
            {
                return result;
            }
            VpkCompiler.Copy(pContainer);

            return null;
        }

        private void runEnd(ModPack pContainer)
        {
            createAndCopy(pContainer.Name);
        }

        public bool Uninstall(string pName)
        {
            var success = removeFile(pName);
            return removeMod(pName) && success;
        }

        public string Install(ModPack pContainer)
        {
            var result = runPrepare(pContainer);
            if(result != null)
            {
                return result;
            }
            var exitCode = VpkCompiler.Run();
            if (exitCode != 0)
                return $"Recieved nonzero exit code from VPK compiler {exitCode}";

            runEnd(pContainer);
            return null;
        }

        public bool SDKInstall(ModPack pContainer, Steam pSteam)
        {
            runPrepare(pContainer);
            if (TrySDKInstalls(pSteam) != 0)
                return false;
            runEnd(pContainer);
            return true;
        }

        private bool removeMod(string pModName)
        {
            try
            {
                Directory.Delete(Path.Combine(_location, "game", pModName), true);
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void createAndCopy(string pModName)
        {
            Directory.CreateDirectory(Path.Combine(_location, "game", pModName));
            var file = new FileInfo(VpkCompiler.VpkOutputFile);
            int trys = 0;
            while (file.Exists == false)
            {
                Thread.Sleep(100);
                trys++;
                if (trys > 20)
                    throw new Exception("VPK file could not be found, but script returned successfully!");
                file = new FileInfo(VpkCompiler.VpkOutputFile);
            }
            File.Copy(VpkCompiler.VpkOutputFile, Path.Combine(_location, "game", pModName, VpkCompiler.VPK_TARGET), true);
        }

        public Dota(string pLocation) : base(pLocation)
        {
        }

        public override bool Validate()
        {
            return base.Validate() && File.Exists(GameInfoPath());
        }
    }
}
