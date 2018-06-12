using System.Configuration;
using System.Linq;

namespace DotaInstaller.src.Utilities
{
    public static class Config
    {
        public static void Set(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains(key))
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
