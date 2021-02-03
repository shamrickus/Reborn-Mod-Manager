using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Core.Mod;

namespace RebornModManager.ModelViews
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

        public List<Core.Mod.Mod> Mods => ModContainer.Mods;
        private List<ModMV> _vMods = null;

        public List<ModMV> VMods
        {
            get { return _vMods ??= ModContainer.Mods.Select(mod => new ModMV(mod)).ToList(); }
            set { _vMods = value; }
        }

        public ModPackMV()
        {
            ModContainer = ModConfiguration.Read();
        }

        public List<Core.Mod.Mod> ActiveMods => Mods.Where(mod => mod.Selected).ToList();
        public bool Error { get; set; }

        public void AddOrUpdate()
        {
            OnPropertyChanged(nameof(Enabled));
            OnPropertyChanged(nameof(InstallText));
        }

        public bool Install()
        {
            var steam = SteamInstance.Get();
            var dota = steam.GetGame(AppIDs.DOTA2_ID);
            return dota.Install(ModContainer);
        }

        public string InstallText => $"Install" + (ActiveMods.Any() ? $" ({ActiveMods.Count})" : "");

        public bool Enabled => ActiveMods.Any();
    }

    public static class ModConfiguration
    {
        public static ModPack Read()
        {
            return Core.Utilities.ReadModConfig(Path.Join(Core.Utilities.AssemblyDirectory(), "config.xml"));
        }
    }
}
