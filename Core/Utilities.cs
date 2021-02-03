using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Core.Mod;

namespace Core
{
    public static class Utilities
    {
        public static bool IsPlatform(OSPlatform pPlatform)
        {
            return RuntimeInformation.IsOSPlatform(pPlatform);
        }

        public static ModPack ReadModConfig(string pConfigFile)
        {
            XmlSerializer s = new XmlSerializer(typeof(ModPack));
            ModPack mods;
            using (var stream = new StringReader(File.ReadAllText(pConfigFile)))
            {
                mods = (ModPack)s.Deserialize(stream);
            }
            return mods;
        }

        public static Version AppVersion()
        {
            return new Version(1, 0, 0);
        }

        public static string AssemblyDirectory() =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string RunShellCommand(string pArguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{pArguments}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var res = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return res;
        }

        public static Process RunScript(string pShellScript, string pArguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(pShellScript)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = pArguments
                }
            };
            process.Start();
            return process;
        }

    }
}
