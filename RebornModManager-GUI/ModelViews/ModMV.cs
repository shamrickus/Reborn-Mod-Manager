using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using RebornModManager.Providers;

namespace RebornModManager.ModelViews
{
    public sealed class ModMV : Observable
    {
        private Core.Mod.Mod _mod;
        private DispatcherTimer _timer;
        private double _timeLeft;

        public Core.Mod.Mod Mod
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
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(SampleFile));
                OnPropertyChanged(nameof(StopPlayText));
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
                OnPropertyChanged(nameof(CardElevation));
            }
        }

        public bool Offensive => Mod.Offensive;
        public Visibility ReplacesCosmetic => Mod.Files.Any(file => file.CosmeticItem) ? Visibility.Visible : Visibility.Hidden;

        public Elevation CardElevation => Mod.Selected ? Elevation.Dp16 : Elevation.Dp0;

        public ModMV(Core.Mod.Mod pMV) : this ()
        {
            Mod = pMV;
        }

        public string StopPlayText
        {
            get
            {
                if (IsPlaying)
                    return $"{_timeLeft.ToString("0.0")}s";
                else
                    return "";
            }
        }

        public double RemainingTime { get; set; }

        public bool IsEnabled
        {
            get
            {
                if (IsPlaying)
                    return true;
                else if (AudioManager.CurrentlyPlaying)
                    return false;
                else return true;
            }
        }

        private bool PlayFile()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Send);
            double totalTime = 0;
            _timer.Tick += (o, e) =>
            {
                _timeLeft = totalTime - AudioManager.GetElapsedSeconds;
                RemainingTime = Math.Round(100 *AudioManager.GetElapsedSeconds / totalTime, 0);
                if (Math.Round(_timeLeft, 1) <= 0 || !IsPlaying)
                    StopPlaying();
                OnPropertyChanged(nameof(StopPlayText));
                OnPropertyChanged(nameof(RemainingTime));
            };
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            bool play = AudioManager.PlayFile($@"{Directory.GetCurrentDirectory()}\{SampleFile}");
            if (play)
            {
                OnPropertyChanged(nameof(GetIcon));
                _timeLeft = AudioManager.GetLengthInSeconds();
                totalTime = _timeLeft;
                _timer.Start();
            }
            else _timeLeft = 0;
            return play;
        }

        public string GetIcon
        {
            get => IsPlaying ? "Stop" : "Play";
        }

        private void StopPlaying()
        {
            _timeLeft = 0;
            RemainingTime = 0;
            _timer.Stop();
            AudioManager.Stop();
            OnPropertyChanged(nameof(RemainingTime));
            OnPropertyChanged(nameof(GetIcon));
        }

        public bool IsPlaying
        {
            get => AudioManager.CurrentlyPlaying && AudioManager.PlayingFile(Mod.SampleFile);
        }


        public void Toggle()
        {
            OnPropertyChanged(nameof(IsPlaying));
            if (IsPlaying)
                StopPlaying();
            else if (!AudioManager.CurrentlyPlaying)
                PlayFile();
        }
    }
}
