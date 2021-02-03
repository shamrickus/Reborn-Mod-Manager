using System.Collections.Generic;
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

        public bool Selected { get; set; }

        public void Copy(string pDestination)
        {
            foreach (var file in Files)
            {
                file.Copy(pDestination);
            }
        }
    }
}
