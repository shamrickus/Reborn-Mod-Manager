using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using DotaInstaller.Properties;
using DotaInstaller.src.ModPack;
using DotaInstaller.src.Services;
using DotaInstaller.src.Utilities;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;

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
            Utilities.RegisterException();
            if (!Utilities.CheckForAdmin())
            {
                MessageBox.Show("Please run the DotaInstaller as admin!");
                Environment.Exit(1);
            }

            _updater = new Updater();
            ResizeMode = ResizeMode.NoResize;

            DataContext = this;
            InitializeComponent();

            Dota2Tome.LocationChanged += () =>
            {
                OnPropertyChanged(nameof(LocationColor));
                OnPropertyChanged(nameof(Location));
            };
            Dota2Tome.SteamLocation = ConfigurationManager.AppSettings[nameof(Dota2Tome.SteamLocation)];
            d2.Title += " V" + ConfigurationManager.AppSettings[nameof(CurrentVersion)];

            _updater.Run();
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

        public ModPack Mods { get; set; }

        public void CheckForUpdate(object sender, RoutedEventArgs e)
        {
            if (CheckForUpdates())
                MessageBox.Show("No new updates found.", "Update Check");
        }

        public bool CheckForUpdates()
        {
            var version = _updater.CheckForUpdate();
            CurrentVersion = VersionManager.BuildFromString(ConfigurationManager.AppSettings["CurrentVersion"]);
            if (version != null && CurrentVersion != null && VersionManager.BuildFromString(version.TagName) > CurrentVersion)
            {
                if (MessageBox.Show($"Update {version.TagName} found, would you like to update?", "Update?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    primaryContent.Visibility = Visibility.Hidden;
                    progressBar.Visibility = Visibility.Visible;
                    _updater.Update(progressBar, version);
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