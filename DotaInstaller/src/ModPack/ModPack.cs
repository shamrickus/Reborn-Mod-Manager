using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DotaInstaller.src.ModPack
{
    [XmlRoot("ModPack")]
    public class ModPack
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Mod")]
        public List<Mod.Mod> Mods { get; set; }

        public void Copy()
        {
            foreach (var mod in Mods.Where(mod => mod.Selected))
            {
                mod.Copy();
            }
        }
    }
}