using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Core.Mod
{
    [XmlRoot("ModPack")]
    public class ModPack
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Mod")]
        public List<Mod> Mods { get; set; }

        public void Copy(string pDestination)
        {
            foreach (var mod in Mods.Where(mod => mod.Selected))
            {
                mod.Copy(pDestination);
            }
        }

        public bool Validate()
        {
            foreach(var mod in Mods)
            {
                if (!mod.Validate())
                    return false;
            }
            return true;
        }
    }
}