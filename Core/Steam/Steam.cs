using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
//Still works on non windows systems, all corresponding methods will throw an exception when called
using Microsoft.Win32;

namespace Core
{
    public static class SteamInstance
    {
        private static Steam _steam;
        public static Steam Get()
        {
            if (_steam == null)
            {
                _steam = new Steam();
            }
                
            return _steam;
        }
    }
    
    public class Steam
    {
        private string _steamBaseLocation;
        private List<string> _otherSteamLocations;
        private Dictionary<string, Dota> _appIdToProduct;

        internal Steam()
        {
            Reset();
        }

        public void Reset()
        {
            _appIdToProduct = new Dictionary<string, Dota>();
        }


        public bool LocateSteam(string pDefault)
        {
            if (!string.IsNullOrEmpty(_steamBaseLocation))
                return true;

            if (Utilities.IsPlatform(OSPlatform.Windows))
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
                _steamBaseLocation = key?.GetValue("InstallPath").ToString();
                return !string.IsNullOrEmpty(_steamBaseLocation);
            }
            else
            {
                var homeDir = Environment.GetEnvironmentVariable("HOME");
                if (homeDir != null)
                {
                    var steamPath = Path.Combine(homeDir, ".steam", "steam");
                    if (Directory.Exists(steamPath))
                    {
                        _steamBaseLocation = steamPath;
                        return true;
                    }
                    
                    steamPath = Path.Combine(homeDir, ".local", "share", "Steam");
                    if (Directory.Exists(steamPath))
                    {
                        _steamBaseLocation = steamPath;
                        return true;
                    }
                }
            }

            if (pDefault != null && Directory.Exists(pDefault))
            {
                _steamBaseLocation = pDefault;
                return true;
            }

            return false;
        }

        public string BaseLocation => _steamBaseLocation;

        public bool LoadAllSteamLibraries()
        {
            _otherSteamLocations = new List<string>();
            var path = Path.Combine(_steamBaseLocation, "steamapps", "libraryfolders.vdf");
            if (File.Exists(path))
            {
                string line;
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    var reader = new StreamReader(file);
                    var found = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (found && line.Contains("path"))
                        {
                            _otherSteamLocations.Add(line.Split("\t").Last().Replace("\"", "").Replace("\\\\", "/"));
                        }
                        else if (line.ToLower().Contains("contentstatsid"))
                        {
                            found = true;
                        }
                    }
                }

            }
            return false;
        }

        private string gameAtLocation(string pLocation, string pAppId)
        {
            var path = Path.Combine(pLocation, "steamapps", $"appmanifest_{pAppId}.acf");
            if (File.Exists(path))
            {
                return path;
            }

            return null;
        }

        private Dictionary<string, Dota> indexGames(string pLocation)
        {
            var games = new Dictionary<string, Dota>();
            var path = Path.Combine(pLocation, "steamapps");
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "appmanifest*.acf"))
                {
                    var appId = file.Split("_").Last().Split(".").First();
                    if (AppIDs.AllAppIDs().Contains(appId))
                        games[appId] = getManifestDirectory(Path.Combine(path, $"appmanifest_{appId}.acf"), pLocation);
                }
            }

            return games;
        }

        private Dota getManifestDirectory(string pManifestFile, string pSteamLocation)
        {
            string line;
            using (var file = File.Open(pManifestFile, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(file);
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("installdir"))
                    {
                        var name = line.Split('\t').Last().Trim('\"');
                        var gameFolder = Path.Combine(pSteamLocation, "steamapps", "common", name);
                        if (Directory.Exists(gameFolder))
                            return new Dota(gameFolder);
                        break;
                    }
                }
            }

            return null;
        }

        private void mergeGameIndex(Dictionary<string, Dota> pGameIndex)
        {
            foreach (var kvp in pGameIndex)
            {
                if (!_appIdToProduct.ContainsKey(kvp.Key))
                    _appIdToProduct[kvp.Key] = kvp.Value;
                else
                    Console.WriteLine($"Duplicate found for {kvp.Key}, {kvp.Value} vs {_appIdToProduct[kvp.Key]}");
            }
        }

        public void IndexAllGames()
        {
            _appIdToProduct = new Dictionary<string, Dota>();
            mergeGameIndex(indexGames(_steamBaseLocation));
            foreach (var folder in _otherSteamLocations)
            {
                mergeGameIndex(indexGames(folder));
            }

            //foreach (var kvp in _appIdToLocation)
            //    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

        public Dota GetGame(string pAppId)
        {
            return _appIdToProduct.ContainsKey(pAppId) ? _appIdToProduct[pAppId] : null;
        }

        public void SetGame(string pAppId, Dota pGame)
        {
            _appIdToProduct[pAppId] = pGame;
        }

        public string LocateGame(string pAppId)
        {
            return _appIdToProduct.ContainsKey(pAppId) ? _appIdToProduct[pAppId].Location : null;
        }
    }
}
