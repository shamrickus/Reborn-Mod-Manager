using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using DotaInstaller.DotaModel;
using DotaInstaller.Mod;
using DotaInstaller.Properties;

namespace DotaInstaller.ModPack
{
    public class ModPackMV : Observable
    {
        private ModPack _modPack;

        public ModPack ModContainer
        {
            get { return _modPack; }
            set
            {
                _modPack = value;
                OnPropertyChanged(nameof(ModPack));
            }
        }

        public List<Mod.Mod> Mods => ModContainer.Mods;
        private List<ModMV> _vMods = null;

        public List<ModMV> VMods
        {
            get { return _vMods ?? (_vMods = ModContainer.Mods.Select(mod => new ModMV(mod)).ToList()); }
            set { _vMods = value; }
        }

        public ModPackMV()
        {
            ModContainer = ModConfiguration.Read();
            Dota2Tome.LocationChanged += () => OnPropertyChanged(nameof(Enabled));
        }

        public List<Mod.Mod> ActiveMods => Mods.Where(mod => mod.Selected).ToList();
        public bool Error { get; set; }

        public void AddOrUpdate()
        {
            OnPropertyChanged(nameof(Enabled));
        }

        public void Install()
        {
            Dota2Tome.Install(ModContainer);
        }

        public bool Enabled => ActiveMods.Any() && !Dota2Tome.Error;
    }

    public static class ModConfiguration
    {
        public static ModPack Read()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            XmlSerializer s = new XmlSerializer(typeof(ModPack));
            using (var stream = new StringReader(Resources.config))
            {
                return (ModPack)s.Deserialize(stream);
            }
        }
    }
}
