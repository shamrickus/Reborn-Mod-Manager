using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace DotaInstaller
{
    public class Updater
    {
        public string EndPoint;
        public static string VERSION_URL = "version.xml";
        public static string DOWNLOAD_FILE = "setup.msi";
        public Uri VersionUri => new Uri(EndPoint + VERSION_URL);
        public Uri DownloadUri => new Uri(EndPoint + DOWNLOAD_FILE);

        public Updater(string pEndPoint)
        {
            EndPoint = pEndPoint;
        }

        public Version CheckForUpdate()
        {
            try
            {
                var request = WebRequest.Create(VersionUri);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var xml = new XmlSerializer(typeof(Version));
                    return (Version)xml.Deserialize(reader);
                }
            }
            catch
            {
                return null;
            }
        }

        public async void Update(ProgressBar pProgressBar)
        {
            var client = new WebClient();
            client.DownloadProgressChanged += (pSender, pArgs) => pProgressBar.Value = pArgs.ProgressPercentage;
            await client.DownloadFileTaskAsync(DownloadUri.ToString(), DOWNLOAD_FILE);

            System.Diagnostics.Process.Start($"{Directory.GetCurrentDirectory()}\\{DOWNLOAD_FILE}");

            Environment.Exit(0);
        }
    }

    [XmlRoot("Version")]
    public class Version
    {
        [XmlElement("Number")]
        public string Number { get; set; }

        [XmlElement("Changes")]
        public string Changes { get; set; }
    }
}