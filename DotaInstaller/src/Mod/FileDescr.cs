using System.IO;
using System.Xml.Serialization;

namespace DotaInstaller.Mod
{
    public class FileDescr
    {
        [XmlElement("Path")]
        public string Path { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        public void Copy()
        {
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{VpkCompiler.VPK_DIR}\{Path}");
            File.Copy($@"{Directory.GetCurrentDirectory()}\{SourcePath}", $@"{Directory.GetCurrentDirectory()}\{VpkCompiler.VPK_DIR}\{Path}\{Name}", true);
        }
    }
}