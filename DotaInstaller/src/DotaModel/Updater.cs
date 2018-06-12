using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using Octokit;

namespace DotaInstaller.src.Utilities
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

        public void Run()
        {
            var worker = new SyncThread();
            worker.Register(() =>
            {
                this.CheckForUpdate();
                return null;
            });
            worker.RunAsync();
        }

        public Release CheckForUpdate()
        {
            try
            {
                var release = Client.Repository.Release.GetAll("shamrickus", "Reborn-Mod-Manager");
                return release.Result[0];
            }
            catch
            {
                return null;
            }
        }

        public async void Update(ProgressBar pProgressBar, Release version)
        {
            var file = version.Assets.First(asset => asset.Name == "Setup.msi");
            var client = new WebClient();
            client.DownloadProgressChanged += (pSender, pArgs) => pProgressBar.Value = pArgs.ProgressPercentage;
            await client.DownloadFileTaskAsync(file.BrowserDownloadUrl, DOWNLOAD_FILE);

            System.Diagnostics.Process.Start($"{Directory.GetCurrentDirectory()}\\{DOWNLOAD_FILE}");

            Environment.Exit(0);
        }
    }
}