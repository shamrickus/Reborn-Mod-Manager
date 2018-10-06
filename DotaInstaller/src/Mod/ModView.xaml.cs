using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DotaInstaller.Providers;

namespace DotaInstaller.Mod
{
    /// <summary>
    /// Interaction logic for ModView.xaml
    /// </summary>
    public partial class ModView : UserControl 
    {
        public static readonly DependencyProperty ModProperty = DependencyProperty.Register("ViewModel", typeof(ModMV), typeof(ModView), new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender, OnModChanged));

        private static void OnModChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var modView = d as ModView;
            modView?.OnModChanged(e);
        }

        private void OnModChanged(DependencyPropertyChangedEventArgs e)
        {
            ViewModel = e.NewValue as ModMV;
        }

        public ModMV ViewModel
        {
            get
            {
                return (ModMV) GetValue(ModProperty);
            }
            set
            {
                DataContext = value;
                SetValue(ModProperty, value);
            }
        }

        public ModView()
        {
            InitializeComponent();
        }

        private void SampleClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Toggle();
        }


        private void Toggle(object sender, EventArgs args)
        {
            ViewModel.Toggle();
        }
    }
}
