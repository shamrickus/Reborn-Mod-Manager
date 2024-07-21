using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Core.Mod
{
    public class Mod
    {
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("SampleFile")]
        public string SampleFile { get; set; }

        [XmlElement("FileDescr")]
        public List<FileDescr> Files { get; set; }

        [XmlElement("Offensive")]
        public bool Offensive { get; set; }

        public bool Selected { get; set; }

        public void Copy(string pDestination)
        {
            foreach (var file in Files)
            {
                file.Copy(pDestination);
            }
        }

        public bool Validate()
        {
            foreach(var file in Files)
            {
                if (!file.Exists())
                    return false;
            }
            if (!File.Exists(System.IO.Path.Join(Utilities.AssemblyDirectory(), SampleFile)))
                return false;
            return true;
        }
    }
}
