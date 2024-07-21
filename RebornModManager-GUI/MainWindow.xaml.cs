using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Core;
using RebornModManager.Providers;
using RebornModManager.Utilities;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace RebornModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        public MainWindow()
        {
            Utilities.Utilities.RegisterException();

            _updater = new Updater();
            CheckForUpdates();
            ResizeMode = ResizeMode.NoResize;

            _steam = SteamInstance.Get();
            var steamFound = _steam.LocateSteam(null);
            if (steamFound)
            {
                _steam.LoadAllSteamLibraries();
                _steam.IndexAllGames();
                _dota = _steam.GetGame(AppIDs.DOTA2_ID);
            }
            else
            {
                _dota = new Dota("invalid location");
            }

            DataContext = this;
            InitializeComponent();

            d2.Title += " V" + _updater.GetAppVersion().ToString();

            BringToFront();
            OnPropertyChanged(nameof(LocationColor));
            OnPropertyChanged(nameof(Location));

            if(!steamFound)
            {
                Dialog.ShowInfo("Steam", "Unable to find steam automatically");
            }
            else if(!_dota.Validate())
            {
                Dialog.ShowInfo("Dota", "Dota was unable to be found from Steam");
            }
        }


        public void BringToFront()
        {
            Topmost = true;
            Thread.Sleep(100);
            Topmost = false;
        }

        public string Location => _dota.Location;

        public Brush LocationColor => _dota.Validate() ? Brushes.White : Brushes.Red;

        public bool DeleteAvailable => _dota.Validate();

        public Visibility NeedToLocate => _dota.Validate() ? Visibility.Hidden : Visibility.Visible;
        public Visibility NoNeedToLocate => _dota.Validate() ? Visibility.Visible : Visibility.Hidden;


        public event PropertyChangedEventHandler PropertyChanged;

        private Updater _updater;
        private Steam _steam;
        private Dota _dota;

        public void CheckForUpdate(object sender, RoutedEventArgs e)
        {
            if (CheckForUpdates())
                Dialog.ShowInfo("Update Check", "No new updates found.");
        }

        public bool CheckForUpdates()
        {
            if(_updater.CheckForUpdate())
            {
                Dialog.ShowInfo("Update Available", _updater.GetChangeLog());
                return false;
            }

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RemoveMods(object sender, RoutedEventArgs e)
        {
            var modConfig = Core.Utilities.ReadModConfig();
            try
            {
                if(!_dota.Uninstall(modConfig.Name))
                    Dialog.ShowInfo("Success", "The mod was uninstalled successfully, however parts of it were not found");
                else
                    Dialog.ShowInfo("Success", "Successfully removed all mods");
            } catch (Exception exp)
            {
                Dialog.ShowInfo("Error", "An error occurred while attempting to uninstall the mods: " + exp.Message + "\nYou can verify the Dota 2 Integrity through steam and this will also remove all the mod files.");
            }
        }

        public bool IncludeOffensive => modPackView?.ViewModel.IncludeOffensive ?? true;

        public void LocateDota(object sender, RoutedEventArgs e)
        {
            var result = Dialog.FolderBrowser("Dota 2 location (steam/steamapps/common)", _steam.BaseLocation);
            if (result == null) return;
            _dota = new Dota(result);
            if (!_dota.Validate())
                _steam.Reset();
            else
                _steam.SetGame(AppIDs.DOTA2_ID, _dota);
            OnPropertyChanged(nameof(LocationColor));
            OnPropertyChanged(nameof(NeedToLocate));
            OnPropertyChanged(nameof(NoNeedToLocate));
            OnPropertyChanged(nameof(Location));
            OnPropertyChanged(nameof(DeleteAvailable));
        }

        private void VolumnChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AudioManager.ChangeVolumn(Convert.ToSingle(e.NewValue / 100));
        }

        private void IncludeOffensive_Changed(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if(modPackView != null)
                modPackView.ViewModel.IncludeOffensive = (e.Source as CheckBox)?.IsChecked ?? true;
        }

        private void modPackView_Loaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(IncludeOffensive));
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            if (modPackView != null)
                modPackView.ViewModel.FilterText = (e.Source as TextBox).Text;
        }
    }
}
