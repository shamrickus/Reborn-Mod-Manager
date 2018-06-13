using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DotaInstaller.src.Mod;

namespace DotaInstaller.src.ModPack
{
    /// <summary>
    /// Interaction logic for ModPackView.xaml
    /// </summary>
    public partial class ModPackView : UserControl
    {
        public ModPackMV ViewModel => (DataContext as ModPackMV);

        public ModPackView()
        {
            DataContext = new ModPackMV(); 
            InitializeComponent();

            foreach (var mod in ViewModel.VMods)
            {
                mod.PropertyChanged += OnChildChecked;
            }
        }

        public void Install(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Error)
            {
                MessageBox.Show("Cant install, Dota 2 directory is invalid");
                return;
            }

            ViewModel.Install();
            MessageBox.Show(
                $"Mods installed successfully!{Environment.NewLine}" +
                $"Start Dota 2 to start using them{Environment.NewLine}" +
                $"Note: Mods will need to be reinstalled after every update");
        }

        public void OnChildChecked(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ModMV.Selected))
            {
                ViewModel.AddOrUpdate();
            }
        }

        public void SelectAll(object sender, RoutedEventArgs e)
        {
            foreach (var mod in ViewModel.VMods)
            {
                mod.Selected = true;
            }
        }
    }
}
