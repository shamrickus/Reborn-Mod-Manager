using System;
using System.Linq;
using System.Net;
using DotaInstaller.Utilities;
using Octokit;

namespace DotaInstaller.DotaModel
{
    public class Updater
    {
        public static string DOWNLOAD_FILE = "setup.msi";
        public static string GIT_HEADER = "Reborn_Mod_Manager";
        private readonly GitHubClient Client;

        public Updater()
        {
            Client = new GitHubClient(new ProductHeaderValue(GIT_HEADER));
        }

        public Release CheckForUpdate()
        {
            try
            {
                var release = Client.Repository.Release.GetLatest("shamrickus", "Reborn-Mod-Manager");
                return release.Result;
            }
            catch
            {
                return null;
            }
        }

        public string GetChangeLog(Release pRelease)
        {
            return
                $"Version {pRelease.TagName} found{Environment.NewLine}{Environment.NewLine}" +
                $"Changes:{Environment.NewLine}" +
                $"{pRelease.Body}{Environment.NewLine}{Environment.NewLine}" +
                $"Would you like to update?";
        }

        public async void Update(Release version)
        {
            var file = version.Assets.First(asset => asset.Name == "Setup.msi");
            using (var client = new WebClient())
            {
                var update = new UpdateDialog("Update", "Downloading...");
                update.Show();
                client.DownloadProgressChanged += (sender, args) => update.Update(args.ProgressPercentage);
                var downloader = client.DownloadFileTaskAsync(file.BrowserDownloadUrl, DOWNLOAD_FILE);
                await downloader.ContinueWith(t => update.Cancel());
            }

#if Release
            System.Diagnostics.Process.Start($"{Directory.GetCurrentDirectory()}\\{DOWNLOAD_FILE}");

            Environment.Exit(0);
#endif
        }
    }
}