using System.Diagnostics;
using System.IO;

namespace DotaInstaller
{
    internal static class VpkCompiler
    {
        public const string VPK_FILE = "vpk.exe";
        public const string VPK_DIR = "pak01_dir";
        public const string VPK_COMP = "pak01_dir.vpk";

        static VpkCompiler()
        {
            Clean();
            Create();
        }

        public static void Run()
        {
            var info = new ProcessStartInfo(VPK_FILE);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.Arguments = VPK_DIR;
            Process.Start(info);
        }

        public static void Create()
        {
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{VPK_DIR}");
        }

        public static void Clean()
        {
            var dir = new DirectoryInfo($@"{Directory.GetCurrentDirectory()}\{VPK_DIR}");
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                    file.Delete();
                foreach (var directory in dir.GetDirectories())
                    directory.Delete(true);
            }
            var vpk = new FileInfo($@"{Directory.GetCurrentDirectory()}\{VPK_COMP}");
            if(vpk.Exists)
                vpk.Delete();
        }
    }
}