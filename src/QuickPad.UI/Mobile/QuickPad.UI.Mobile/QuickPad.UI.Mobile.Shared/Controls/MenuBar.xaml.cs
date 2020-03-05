using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using QuickPad.Mvvm.ViewModels;
using QuickPad.Mvvm.Commands;
using QuickPad.Mvvm.Models;
using QuickPad.UI.Helpers;
using QuickPad.UI.Theme;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuickPad.UI.Controls
{
    public sealed partial class MenuBar : UserControl
    {
        public IVisualThemeSelector VtSelector => VisualThemeSelector.Current;

        public SettingsViewModel<StorageFile, IRandomAccessStream> Settings => App.Settings;

        public IQuickPadCommands<StorageFile, IRandomAccessStream> Commands => App.Commands;

        public DocumentViewModel<StorageFile, IRandomAccessStream> ViewModel
        {
            get => DataContext as DocumentViewModel<StorageFile, IRandomAccessStream>;
            set
            {
                if (value == null || DataContext == value) return;

                DataContext = value;
            }
        }

        public DocumentModel<StorageFile, IRandomAccessStream> ViewModelDocument => ViewModel.Document;

        public MenuBar()
        {
            this.InitializeComponent();

            Settings.PropertyChanged += SettingsOnPropertyChanged;
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SettingsViewModel<StorageFile, IRandomAccessStream>.CurrentMode):
#if NETFX_CORE
                    Bindings.Update();
#endif
                    break;
            }
        }
    }
}
