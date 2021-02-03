using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
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
        private Version _currentVersion; 

        public MainWindow()
        {
            Utilities.Utilities.RegisterException();

            _updater = new Updater();
            CheckForUpdates();
            ResizeMode = ResizeMode.NoResize;

            _steam = SteamInstance.Get();
            _steam.IndexAllGames();
            _dota = _steam.GetGame(AppIDs.DOTA2_ID);

            DataContext = this;
            InitializeComponent();

            d2.Title += " V" + _currentVersion;

            BringToFront();
        }


        public void BringToFront()
        {
            Topmost = true;
            Thread.Sleep(100);
            Topmost = false;
        }

        public string Location => _dota.Location;

        public Brush LocationColor => !_dota.Validate() ? Brushes.Red : Brushes.White;

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
                Dialog.ShowInfo("Update Available", _updater.GetChangeLog(version));
                return false;
            }

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            _dota = new Dota(Dialog.FolderBrowser("Dota 2 location (steam/steamapps/common)", _steam.BaseLocation));
            OnPropertyChanged(nameof(LocationColor));
            OnPropertyChanged(nameof(Location));
        }

        private void VolumnChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AudioManager.ChangeVolumn(Convert.ToSingle(e.NewValue / 100));
        }
    }
}
