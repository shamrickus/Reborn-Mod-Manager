using System;
using System.Linq;
using NAudio.Wave;

namespace RebornModManager.Providers
{
    public static class AudioManager
    {
        private static WaveOutEvent _device = new WaveOutEvent();
        private static AudioFileReader _file;
        private static TimeSpan _duration = TimeSpan.Zero;
        public static bool CurrentlyPlaying => _device.PlaybackState == PlaybackState.Playing;

        static AudioManager()
        {
            _device.PlaybackStopped += (sender, args) =>
            {
                PlaybackChanged?.Invoke();
                _duration = TimeSpan.Zero;
            };
        }

        public static bool PlayFile(string pFile)
        {
            _file = new AudioFileReader(pFile);
            if (!CurrentlyPlaying)
            {
                try
                {
                    _duration = _file.TotalTime;
                    _device.Init(_file);
                    _device.Play();
                    PlaybackChanged?.Invoke();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public static double GetLengthInSeconds()
        {
            return _duration.TotalSeconds;
        }

        public static void ChangeVolumn(float pValue)
        {
            _device.Volume = pValue;
        }

        public delegate void OnPlaybackChanged();
        public static event OnPlaybackChanged PlaybackChanged;

        public static bool PlayingFile(string pSampleFile)
        {
            if(_file != null)
                return _file.FileName.Split('\\').Last() == pSampleFile.Split('\\').Last();
            return false;
        }

        public static void Stop()
        {
            if(CurrentlyPlaying)
                _device.Stop();
        }
    }
}
