using System;
using System.IO;
using System.Runtime.InteropServices;
using Core.Mod;

namespace Core
{
    public static class VpkCompiler
    {
        private const string VPK_DIR = "pak01_dir";
        private const string VPK_COMP = "pak01.vpk";
        public const string VPK_TARGET = "pak01_dir.vpk";

        private static string VPKBinDir => Path.Join(Utilities.AssemblyDirectory(), "VPK");
        
        public static string VPKBinDirectory()
        {
            if (Utilities.IsPlatform(OSPlatform.Windows))
                return Path.Join(VPKBinDir, "windows");
            else if (Utilities.IsPlatform(OSPlatform.Linux))
                return Path.Join(VPKBinDir, "linux");
            else
                throw new Exception("Unsupported OS");
        }

        public static string VPKExecutable()
        {
            string vpkFile = VPKBinDirectory();
            if (Utilities.IsPlatform(OSPlatform.Windows))
            {
                vpkFile = Path.Join(vpkFile, "vpk.exe");
            }
            else if (Utilities.IsPlatform(OSPlatform.Linux))
            {
                vpkFile = Path.Join(vpkFile, "vpk.sh");
                Console.WriteLine(
                    Utilities.RunShellCommand($"chmod +x {vpkFile}"));
            }
            else
                throw new Exception("Unsupported OS");

            return vpkFile;
        }

        public static string VPKArgs(string pVPKDirectory)
        {
            if (Utilities.IsPlatform(OSPlatform.Windows))
                return VpkDirectory;
            else
                return "\"" + pVPKDirectory + "\" " + VpkDirectory;
        }
        
        public static string VpkDirectory => Path.Join(VPKBinDir, VPK_DIR);

        public static string VpkOutputFile => Utilities.IsPlatform(OSPlatform.Windows)
            ? Path.Join(VPKBinDir, VPK_TARGET)
            : Path.Join(VPKBinDir, VPK_COMP);

        public static int Run(string pVPKDirectory=null)
        {
            if (pVPKDirectory == null)
                pVPKDirectory = VPKBinDirectory();
            
            var vpkProcess = Utilities.RunScript(VPKExecutable(), VPKArgs(pVPKDirectory));
            vpkProcess.WaitForExit();
            var stdOut = vpkProcess.StandardOutput.ReadToEnd();
            if(!string.IsNullOrEmpty(stdOut)) 
                Console.WriteLine($"VPK stdout: {stdOut}");
            var stdErr = vpkProcess.StandardError.ReadToEnd();
            if(!string.IsNullOrEmpty(stdErr))
                Console.WriteLine($"VPK stderr: {stdErr}");
            return vpkProcess.ExitCode;
        }

        public static void Create()
        {
            Directory.CreateDirectory(VpkDirectory);
        }

        public static void Copy(ModPack pContainer)
        {
            pContainer.Copy(VpkDirectory);
        }

        public static void Clean()
        {
            var dir = new DirectoryInfo(VpkDirectory);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                    file.Delete();
                foreach (var directory in dir.GetDirectories())
                    directory.Delete(true);
            }
            var vpk = new FileInfo(Path.Join(VpkOutputFile));
            if(vpk.Exists)
                vpk.Delete();
        }
    }
}
