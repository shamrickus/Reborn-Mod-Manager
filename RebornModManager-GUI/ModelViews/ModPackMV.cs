using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Core.Mod;
using RebornModManager.Utilities;

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
        private Steam _steam;

        public List<ModMV> VMods
        {
            get { return _vMods ??= ModContainer.Mods.Select(mod => new ModMV(mod)).ToList(); }
            set { _vMods = value; }
        }

        public ModPackMV()
        {
            ModContainer = Core.Utilities.ReadModConfig();
            _steam = SteamInstance.Get();
        }

        public List<Core.Mod.Mod> ActiveMods => Mods.Where(mod => mod.Selected).ToList();
        public bool Error { get; set; }

        public void AddOrUpdate()
        {
            OnPropertyChanged(nameof(Enabled));
            OnPropertyChanged(nameof(InstallText));
        }

        public string Install()
        {
            var dota = _steam.GetGame(AppIDs.DOTA2_ID);
            if (dota == null)
            {
                return "Dota location is invalid";
            }
            return dota.Install(ModContainer);
        }

        public string InstallText => $"Install" + (ActiveMods.Any() ? $" ({ActiveMods.Count})" : "");

        public bool Enabled
        {
            get {
                var dota = _steam.GetGame(AppIDs.DOTA2_ID);
                return ActiveMods.Any() && dota != null && dota.Validate();
            }
        }
    }
}
