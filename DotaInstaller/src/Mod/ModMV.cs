using System.IO;
using System.Windows;
using DotaInstaller.Providers;

namespace DotaInstaller.Mod
{
    public sealed class ModMV : Observable
    {
        private Mod _mod;
        public Mod Mod
        {
            get { return _mod; }
            set
            {
                _mod = value;
                OnPropertyChanged(nameof(Mod));
            }
        }

        public string SampleFile => Mod.SampleFile;

        public ModMV()
        {
            AudioManager.PlaybackChanged += () =>
            {
                OnPropertyChanged(nameof(NotPlaying));
                OnPropertyChanged(nameof(CanPlay));
                OnPropertyChanged(nameof(CanStop));
                OnPropertyChanged(nameof(SampleFile));
            };
        }

        public string DisplayName => Mod.DisplayName;
        public string Description => Mod.Description;

        public bool Selected
        {
            get { return Mod.Selected; }
            set
            {
                Mod.Selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public ModMV(Mod pMV) : this ()
        {
            Mod = pMV;
        }

        public Visibility CanPlay
        {
            get
            {
                if (!AudioManager.NotPlaying && AudioManager.PlayingFile(SampleFile))
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
        public Visibility CanStop
        {
            get
            {
                if (AudioManager.NotPlaying || !AudioManager.PlayingFile(SampleFile))
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }


        public bool NotPlaying
        {
            get { return AudioManager.NotPlaying; }
        }

        public bool PlayFile()
        {
            return AudioManager.PlayFile($@"{Directory.GetCurrentDirectory()}\{SampleFile}");
        }
    }
}
