using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
        private List<ModMV> _modMVs;
        private ICollectionView _source;
        private Steam _steam;
        private bool _installing;

        public ICollectionView VMods => _source;

        public ModPackMV()
        {
            ModContainer = Core.Utilities.ReadModConfig();
            if (!ModContainer.Validate())
            {
                Dialog.ShowInfo("Bad Files", "Some files are missing, mods might fail to install.");
            }
            _steam = SteamInstance.Get();
            _modMVs = Mods.Select(mod => new ModMV(mod)).ToList();
            _source = CollectionViewSource.GetDefaultView(_modMVs);
            _source.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
            _source.Filter = Filter;

            var raw = ConfigurationManager.AppSettings["includeOffensive"];
            if (raw != null)
            {
                bool includeOffensive;
                if (bool.TryParse(raw, out includeOffensive))
                {
                    IncludeOffensive = includeOffensive;
                }
            }
        }

        private bool Filter(object pItem)
        {
            var mod = pItem as ModMV;
            if (!IncludeOffensive && mod.Offensive)
                return false;
            if (!(_filter == "" || _filter == null) &&
                FuzzySharp.Fuzz.PartialRatio(mod.DisplayName.ToLower(), _filter) < 70 &&
                FuzzySharp.Fuzz.PartialRatio(mod.Description.ToLower(), _filter) < 70)
                return false;
            return true;
        }

        public IEnumerable<Mod> ActiveMods()
        {
            var active = new List<Mod>();
            foreach(ModMV mv in _modMVs)
            {
                if(mv.Selected)
                {
                    active.Add(mv.Mod);
                }
            }
            return active;
        }

        public bool Error { get; set; }

        public void AddOrUpdate()
        {
            OnPropertyChanged(nameof(Enabled));
            OnPropertyChanged(nameof(InstallText));
        }

        private bool _includeOffensive = true;
        public bool IncludeOffensive
        {
            get => _includeOffensive;
            set
            {
                _includeOffensive = value;
                if (!_includeOffensive)
                {
                    foreach(ModMV mod in _modMVs)
                    {
                        if(mod.Offensive)
                            mod.Selected = false;
                    }
                }
                OnPropertyChanged(nameof(VMods));
                _source.Refresh();
            }
        }

        private string _filter;
        public string FilterText
        {
            get => _filter;
            set
            {
                _filter = value.ToLower();
                _source.Refresh();
            }
        }

        public async Task<string> Install()
        {
            var dota = _steam.GetGame(AppIDs.DOTA2_ID);
            if (dota == null)
            {
                return "Dota location is invalid";
            }
            string result = null;
            _installing = true;
            OnPropertyChanged(nameof(Installing));
            await Task.Run(() =>
            {
                result = dota.Install(ModContainer);
                _installing = false;
                OnPropertyChanged(nameof(Installing));
            });
            return result;
        }

        public Visibility Installing
        {
            get => _installing ? Visibility.Visible : Visibility.Hidden; 
        }

        public string InstallText => $"Install" + (ActiveMods().Any() ? $" ({ActiveMods().Count()})" : "");

        public bool Enabled
        {
            get {
                var dota = _steam.GetGame(AppIDs.DOTA2_ID);
                return ActiveMods().Any() && dota != null && dota.Validate();
            }
        }
    }
}
