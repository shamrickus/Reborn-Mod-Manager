using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace Core
{
    public static class VersionManager
    {
        public static Version BuildFromString(string pVersion)
        {
            var parts = pVersion.Split('.').ToList().Select(Int16.Parse).ToList();
            return new Version(parts[0], parts[1], parts[2]);
        }
    }
    
    public class GithubRelease {
         
        public string tag_name { get; set; }
        public string body { get; set; }

        public Version GetVersion()
        {
            return VersionManager.BuildFromString(tag_name);
        }
    }
    
    public class Updater
    {
        private GithubRelease _latestRelease;

        private void GetWebVersion()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("user-agent" ,"httpClient");
                var response = client.GetAsync("https://api.github.com/repos/shamrickus/Reborn-Mod-Manager/releases").Result;
                var body = response.Content.ReadAsStringAsync().Result;
                var res = JsonSerializer.Deserialize<List<GithubRelease>>(body);
                _latestRelease = res[0];
            }
            catch
            {
            }
        }

        public Version GetAppVersion()
        {
            return Utilities.AppVersion();
        }
        
        public bool CheckForUpdate()
        {
            if(_latestRelease == null)
                GetWebVersion();
            return _latestRelease.GetVersion() > Utilities.AppVersion();
        }

        public string GetChangeLog()
        {
            return
                $"Version {_latestRelease.tag_name} found{Environment.NewLine}{Environment.NewLine}" +
                $"Changes:{Environment.NewLine}" +
                $"{_latestRelease.body}{Environment.NewLine}{Environment.NewLine}";
        }
    }
}
