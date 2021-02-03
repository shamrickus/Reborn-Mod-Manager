using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using RebornModManager.Utilities;

namespace RebornModManager.ModelViews
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
                Dialog.ShowInfo("Info", "Can't install mods, Dota 2 directory is invalid");
                return;
            }

            if (ViewModel.Install())
                Dialog.ShowInfo("Success",
                    $"Mods installed successfully!{Environment.NewLine}" +
                    $"Start Dota 2 to start using them{Environment.NewLine}" +
                    $"Note: Mods may need to be reinstalled after a major update");
            else
                Dialog.ShowInfo("Failure",
                    "Unable to install mods!");
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
