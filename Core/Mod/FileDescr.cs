using System.IO;
using System.Xml.Serialization;

namespace Core.Mod
{
    public class FileDescr
    {
        [XmlElement("Path")]
        public string Path { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }

        private string FullSourcePath => System.IO.Path.Join(Utilities.AssemblyDirectory(), SourcePath);

        [XmlElement("Type")]
        public string Type { get; set; }

        public void Copy(string pDestination)
        {
            Directory.CreateDirectory(System.IO.Path.Join(pDestination, Path));
            File.Copy(FullSourcePath, System.IO.Path.Join(pDestination, Path, Name), true);
        }
    }
}