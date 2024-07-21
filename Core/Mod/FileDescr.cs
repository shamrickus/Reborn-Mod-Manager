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

        [XmlElement("CosmeticItem")]
        public bool CosmeticItem { get; set; }

        private string FullSourcePath => System.IO.Path.Join(Utilities.AssemblyDirectory(), SourcePath + FileExtension());

        [XmlElement("Type")]
        public string Type { get; set; }

        private string FileExtension()
        {
            switch(Type)
            {
                case "Sound":
                    return ".vsnd_c";
            }
            throw new System.Exception($"Unknown FileDescr Type {Type}");
        }

        public void Copy(string pDestination)
        {
            Directory.CreateDirectory(System.IO.Path.Join(pDestination, Path));
            File.Copy(FullSourcePath, System.IO.Path.Join(pDestination, Path, Name + FileExtension()), true);
        }

        public bool Exists()
        {
            return File.Exists(System.IO.Path.Join(FullSourcePath));
        }
    }
}