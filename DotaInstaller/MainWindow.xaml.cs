using DotaInstaller.Annotations;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace DotaInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public WaveOutEvent device = new WaveOutEvent();

        public string SelectedPath;

        private string _steamLocation;

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += (object s, UnhandledExceptionEventArgs args) =>
            {
                using (var writer = new StreamWriter($@"{Directory.GetCurrentDirectory()}\Error.txt"))
                {
                    var excp = args.ExceptionObject as Exception;
#if !DEBUG
                    writer.WriteLine(
                        $"Message: {excp.Message}{Environment.NewLine}Stack Trace: {excp.StackTrace}{Environment.NewLine}");
#else
                    throw new Exception(excp.Message);
#endif
#pragma warning disable CS0162 // Unreachable code detected
                    MessageBox.Show($"An exception has occurred!{Environment.NewLine}Exception: {excp.Message}");
#pragma warning restore CS0162 // Unreachable code detected
                }
            };
            CheckForAdmin();
            var worker = new BGWorker();
            worker.Register(() =>
            {
                CheckForUpdates();
                return null;
            });
            worker.RunAfter(BringToFront);

            InitializeComponent();
            DataContext = this;
            worker.RunAsync();
            ResizeMode = ResizeMode.NoResize;

            VpkCompiler.Create();
            Mods = ModConfiguration.Read();

        }

        public object BringToFront()
        {
            Topmost = true;
            Thread.Sleep(100);
            Topmost = false;
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ModsSelected = false;
        public List<Mod> ActiveMods
        {
            get
            {
                if (Mods == null)
                    return new List<Mod>();
                var mods =  ListOfMods.Where(mod => mod.Selected).ToList();
                ModsSelected = mods.Count > 0;
                return mods;
            }
        }

        public List<Mod> ListOfMods
        {
            get
            {
                if (Mods == null)
                    return new List<Mod>();
                return Mods.Mods;
            }
        }

        public ModList Mods { get; set; }

        public string SteamLocation
        {
            get
            {
                if (string.IsNullOrEmpty(_steamLocation))
                    _steamLocation = ConfigurationManager.AppSettings[nameof(SteamLocation)];
                return _steamLocation;
            }
            set
            {
                _steamLocation = value;
                Set(nameof(SteamLocation), _steamLocation);
                Error = !Dota2Tome.ConfigExists(_steamLocation);
                if (Error)
                    MessageBox.Show("Required files are not able to be found in that location!");
                OnPropertyChanged(nameof(SteamLocation));
            }
        }

        private bool Error { get; set; }

        public static void Set(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains(key))
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }
        public void CheckForUpdate(object sender, RoutedEventArgs e) => CheckForUpdates();

        public void CheckForUpdates()
        {
            var updater = new Updater(ConfigurationManager.AppSettings["UpdateServer"]);
            var version = updater.CheckForUpdate();
            if (version != null)
            {
                if (MessageBox.Show($"Update {version.Number} found, would you like to update?", "Update?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    primaryContent.Visibility = Visibility.Hidden;
                    progressBar.Visibility = Visibility.Visible;
                    updater.Update(progressBar);
                }
            }
            else
                MessageBox.Show("Unable to check for updates!");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateLocation();
        }

        private void CheckForAdmin()
        {
            bool admin;
            using (WindowsIdentity iden = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(iden);
                admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (!admin)
            {
                MessageBox.Show("Please run the DotaInstaller as admin!");
                Environment.Exit(1);
            }
        }
        private void Install(object sender, RoutedEventArgs e)
        {
            if (Error)
            {
                MessageBox.Show("Cant install, Dota 2 directory is invalid");
                return;
            }
            else if (ActiveMods.Count == 0)
            {
                MessageBox.Show("Please select files to install first!");
                return;
            }

            VpkCompiler.Clean();
            Dota2Tome.ReadFile(SteamLocation, Mods.ModName);
            Mods.Copy();
            VpkCompiler.Run();

            Dota2Tome.CreateAndCopy(SteamLocation, Mods.ModName);

            MessageBox.Show(
                $"Mods installed successfully!{Environment.NewLine}" +
                $"Start Dota 2 to start using them{Environment.NewLine}" +
                $"Note: Mods will need to be reinstalled after every update");
        }

        private void SampleClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                AudioFileReader audioFile = new AudioFileReader($@"{Directory.GetCurrentDirectory()}\{btn.Tag}");
                if (device.PlaybackState != PlaybackState.Playing)
                {
                    device.Init(audioFile);
                    try
                    {
                        device.Play();
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("Could not find sample file!");
                    }
                }
            }
        }

        private void UpdateLocation()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Where is your Dota 2 folder located?";
                dialog.SelectedPath = SteamLocation;
                var result = dialog.ShowDialog(this.GetIWin32Window());
                if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                    SteamLocation = dialog.SelectedPath;
            }
        }
        private void VolumnChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            device.Volume = Convert.ToSingle(e.NewValue / 100);
        }
    }
}