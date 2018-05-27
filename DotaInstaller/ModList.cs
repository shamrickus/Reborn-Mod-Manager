using System;
using DotaInstaller.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using DotaInstaller.Annotations;

namespace DotaInstaller
{
    [XmlRoot("ModListUI")]
    public class ModList
    {
        [XmlElement("ModName")]
        public string ModName { get; set; }

        [XmlElement("Mod")]
        public List<Mod> Mods { get; set; }

        public void Copy()
        {
            foreach (var mod in Mods)
            {
                mod.Copy();
            }
        }
    }

    public class Mod : INotifyPropertyChanged
    {
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("SampleFile")]
        public string SampleFile { get; set; }

        [XmlElement("FileDescr")]
        public List<FileDescr> Files { get; set; }

        public bool Selected { get; set; }

        public void Copy()
        {
            foreach (var file in Files)
            {
                file.Copy();
            }
        }

        Mod()
        {
            AudioManager.PlaybackChanged += () => { OnPropertyChanged(nameof(NotPlaying)); OnPropertyChanged(nameof(IsActive)); OnPropertyChanged(nameof(IsInActive));};
            OnPropertyChanged(nameof(NotPlaying)); OnPropertyChanged(nameof(IsActive)); OnPropertyChanged(nameof(IsInActive));
        }

        public System.Windows.Visibility IsActive
        {
            get
            {
                if (AudioManager.PlayingFile(SampleFile) && !AudioManager.NotPlaying)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
        public System.Windows.Visibility IsInActive
        {
            get
            {
                if (!AudioManager.PlayingFile(SampleFile) || AudioManager.NotPlaying)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }

        public bool NotPlaying
        {
            get { return AudioManager.NotPlaying; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FileDescr
    {
        [XmlElement("Path")]
        public string Path { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        public void Copy()
        {
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{VpkCompiler.VPK_DIR}\{Path}");
            File.Copy($@"{Directory.GetCurrentDirectory()}\{SourcePath}", $@"{Directory.GetCurrentDirectory()}\{VpkCompiler.VPK_DIR}\{Path}\{Name}", true);
        }
    }

    public static class ModConfiguration
    {
        public static ModList Read()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            XmlSerializer s = new XmlSerializer(typeof(ModList));
            using (var stream = new StringReader(Resources.config))
            {
                return (ModList)s.Deserialize(stream);
            }
        }
    }
}