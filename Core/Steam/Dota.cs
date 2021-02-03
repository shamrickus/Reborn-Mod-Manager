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

        public void RemoveFile(string pName)
        {
            List<string> fileData = new List<string>();
            string line;
            using (var file = File.Open(GameInfoPath(), FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(file);
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == AddonLine(pName))
                        continue;
                    fileData.Add(line);
                }
            }

            using (var tw = new StreamWriter(GameInfoPath()))
            {
                foreach (var lineData in fileData)
                    tw.WriteLine(lineData);
            }
        }

        public void ReadFile(string pName)
        {
            string line;
            List<string> fileData = new List<string>();
            using (var file = File.Open(GameInfoPath(), FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(file);
                var dotaLv = "\t\t\tGame_LowViolence\tdota_lv";
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
                        fileData.Add(AddonLine(pName));
                        lvFound = true;
                    }
                }
            }

            using (var tw = new StreamWriter(GameInfoPath()))
            {
                foreach (var lineData in fileData)
                    tw.WriteLine(lineData);
            }
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

        private void runPrepare(ModPack pContainer)
        {
            VpkCompiler.Clean();
            VpkCompiler.Create();

            ReadFile(pContainer.Name);
            VpkCompiler.Copy(pContainer);
        }
        
        private void runEnd(ModPack pContainer)
        {
            CreateAndCopy(pContainer.Name);
        }

        public bool Install(ModPack pContainer)
        {
            runPrepare(pContainer);
            if (VpkCompiler.Run() != 0)
                return false;
                
            runEnd(pContainer);
            return true;
        }

        public bool SDKInstall(ModPack pContainer, Steam pSteam)
        {
            runPrepare(pContainer);
            if (TrySDKInstalls(pSteam) != 0)
                return false;
            runEnd(pContainer);
            return true;
        }

        public void CreateAndCopy(string pModName)
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
