using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using DotaInstaller.DotaModel;
using DotaInstaller.Properties;
using DotaInstaller.Providers;
using DotaInstaller.Utilities;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace DotaInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        public string SelectedPath;

        private Version CurrentVersion; 

        public MainWindow()
        {
            Utilities.Utilities.RegisterException();
            if (!Utilities.Utilities.CheckForAdmin())
            {
                Dialog.ShowInfo("Info", "Please run the DotaInstaller as admin!");
                Environment.Exit(1);
            }

            _updater = new Updater();
            CheckForUpdates();
            ResizeMode = ResizeMode.NoResize;

            DataContext = this;
            InitializeComponent();

            Dota2Tome.LocationChanged += () =>
            {
                OnPropertyChanged(nameof(LocationColor));
                OnPropertyChanged(nameof(Location));
            };
            d2.Title += " V" + ConfigurationManager.AppSettings[nameof(CurrentVersion)];

            BringToFront();
        }


        public void BringToFront()
        {
            Topmost = true;
            Thread.Sleep(100);
            Topmost = false;
        }

        public string Location => Dota2Tome.SteamLocation;

        public Brush LocationColor => Dota2Tome.Error ? Brushes.Red : Brushes.White;

        public event PropertyChangedEventHandler PropertyChanged;

        private Updater _updater;

        public ModPack.ModPack Mods { get; set; }

        public void CheckForUpdate(object sender, RoutedEventArgs e)
        {
            if (CheckForUpdates())
                Dialog.ShowInfo("Update Check", "No new updates found.");
        }

        public bool CheckForUpdates()
        {
            var version = _updater.CheckForUpdate();
            CurrentVersion = VersionManager.BuildFromString(ConfigurationManager.AppSettings["CurrentVersion"]);
            if (version != null && CurrentVersion != null && VersionManager.BuildFromString(version.TagName) > CurrentVersion)
            {
                if(Dialog.YesNoBox("Update?", _updater.GetChangeLog(version)) == Dialog.Button.Yes)
                {
                    _updater.Update(version);
                }

                return false;
            }

            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Dota2Tome.UpdateLocation();
        }

        private void VolumnChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AudioManager.ChangeVolumn(Convert.ToSingle(e.NewValue / 100));
        }
    }
}