using System.Linq;
using NAudio.Wave;

namespace DotaInstaller.src.Utilities
{
    public static class AudioManager
    {
        private static WaveOutEvent _device = new WaveOutEvent();
        private static AudioFileReader _file;
        public static bool NotPlaying => _device.PlaybackState != PlaybackState.Playing;

        static AudioManager()
        {
            _device.PlaybackStopped += (sender, args) =>
            {
                PlaybackChanged?.Invoke();
            };
        }

        public static bool PlayFile(string pFile)
        {
            _file = new AudioFileReader(pFile);
            if (NotPlaying)
            {
                _device.Init(_file);
                try
                {
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
            if(!NotPlaying)
                _device.Stop();
        }
    }
}
