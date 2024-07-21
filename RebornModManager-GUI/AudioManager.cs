using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Utils;
using NAudio.Wave;

namespace RebornModManager.Providers
{
    public static class AudioManager
    {
        private static WaveOutEvent _device = new WaveOutEvent();
        private static Dictionary<string, AudioFileReader> _files;
        private static TimeSpan _duration = TimeSpan.Zero;
        private static AudioFileReader _activeFile;
        public static bool CurrentlyPlaying => _device.PlaybackState == PlaybackState.Playing;

        static AudioManager()
        {
            _files = new Dictionary<string, AudioFileReader>();
            _device.PlaybackStopped += (sender, args) =>
            {
                PlaybackChanged?.Invoke();
                _duration = TimeSpan.Zero;
            };
        }

        public static bool PlayFile(string pFile)
        {
            _files.TryGetValue(pFile, out var file);
            if (file == null)
            {
                file = new AudioFileReader(pFile);
                _files.Add(pFile, file);
            } else
            {
                file.Seek(0, System.IO.SeekOrigin.Begin);
            }
            if (!CurrentlyPlaying)
            {
                try
                {
                    _duration = file.TotalTime;
                    _device.Init(file);
                    _activeFile = file;
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

        public static double GetElapsedSeconds => _device.GetPositionTimeSpan().TotalSeconds;

        public static void ChangeVolumn(float pValue)
        {
            _device.Volume = pValue;
        }

        public delegate void OnPlaybackChanged();
        public static event OnPlaybackChanged PlaybackChanged;

        public static bool PlayingFile(string pSampleFile)
        {
            if(_activeFile != null)
                return _activeFile.FileName.Split('\\').Last() == pSampleFile.Split('\\').Last();
            return false;
        }

        public static void Stop()
        {
            if(CurrentlyPlaying)
                _device.Stop();
        }
    }
}
