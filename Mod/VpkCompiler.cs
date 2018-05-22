using System.IO;

namespace DotaInstaller
{
    static class VpkCompiler
    {
        public const string VPK_FILE = "vpk.exe";
        public const string VPK_DIR = "pak01_dir";
        public const string VPK_COMP = "pak01_dir.vpk";

        public static void Run()
        {
            var vpkArgs = VPK_DIR;
            System.Diagnostics.Process.Start(VPK_FILE, vpkArgs);
        }

        public static void Create()
        {
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{VPK_DIR}");
        }

        public static void Clean()
        {
            var dir = new DirectoryInfo($@"{Directory.GetCurrentDirectory()}\{VPK_DIR}");
            foreach (var file in dir.GetFiles())
                file.Delete();
            foreach(var directory in dir.GetDirectories())
                directory.Delete(true);
        }
    }
}
