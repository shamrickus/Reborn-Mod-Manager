using System;
using System.IO;
using System.Windows.Threading;
using RebornModManager.Providers;

namespace RebornModManager.ModelViews
{
    public sealed class ModMV : Observable
    {
        private Core.Mod.Mod _mod;
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
            }
        }

        public ModMV(Core.Mod.Mod pMV) : this ()
        {
            Mod = pMV;
        }

        private DispatcherTimer _timer;
        private double _timeLeft;

        public string StopPlayText
        {
            get
            {
                if (IsPlaying())
                    return $"Stop {_timeLeft.ToString("0.0")}s";
                else
                    return "Play";
            }
        }

        public bool IsEnabled
        {
            get
            {
                if (IsPlaying())
                    return true;
                else if (AudioManager.CurrentlyPlaying)
                    return false;
                else return true;
            }
        }

        private bool PlayFile()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Send);
            DateTime time = DateTime.Now;
            _timer.Tick += (o, e) =>
            {
                var now = DateTime.Now;
                _timeLeft -= (now - time).TotalSeconds;
                time = now;
                if (_timeLeft <= 0)
                    StopPlaying();
                OnPropertyChanged(nameof(StopPlayText));
            };
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            bool play = AudioManager.PlayFile($@"{Directory.GetCurrentDirectory()}\{SampleFile}");
            if (play)
            {
                _timeLeft = AudioManager.GetLengthInSeconds();
                _timer.Start();
            }
            else _timeLeft = 0;
            return play;
        }

        private void StopPlaying()
        {
            _timeLeft = 0;
            _timer.Stop();
            AudioManager.Stop();
        }

        public bool IsPlaying()
        {
            return AudioManager.CurrentlyPlaying && AudioManager.PlayingFile(Mod.SampleFile);
        }


        public void Toggle()
        {
            if (IsPlaying())
                StopPlaying();
            else if (!AudioManager.CurrentlyPlaying)
                PlayFile();
        }
    }
}
