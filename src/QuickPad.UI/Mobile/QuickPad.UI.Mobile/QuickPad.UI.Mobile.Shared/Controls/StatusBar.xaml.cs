using Windows.UI.Xaml.Controls;
using QuickPad.Mvvm.ViewModels;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Streams;
using QuickPad.Mvvm.Models;
using QuickPad.UI.Helpers;
using QuickPad.UI.Theme;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuickPad.UI.Controls
{
    public sealed partial class StatusBar  
    {
        public IVisualThemeSelector VtSelector => VisualThemeSelector.Current;

        public SettingsViewModel<StorageFile, IRandomAccessStream> Settings => App.Settings;

        public DocumentViewModel<StorageFile, IRandomAccessStream> ViewModel
        {
            get => DataContext as DocumentViewModel<StorageFile, IRandomAccessStream>;
            set
            {
                if (value == null || DataContext == value) return;

                if (DataContext is DocumentViewModel<StorageFile, IRandomAccessStream> documentViewModel)
                {
                    documentViewModel.PropertyChanged -= DocumentViewModelOnPropertyChanged;
                }

                DataContext = value;

                value.PropertyChanged += DocumentViewModelOnPropertyChanged;
            }
        }

        public DocumentModel<StorageFile, IRandomAccessStream> ViewModelDocument => ViewModel.Document;

        private void DocumentViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DocumentViewModel<StorageFile, IRandomAccessStream>.CurrentLine):
                case nameof(DocumentViewModel<StorageFile, IRandomAccessStream>.CurrentColumn):
#if NETFX_CORE
                    Bindings.Update();
#endif
                    break;
            }
        }

        public StatusBar()
        {
            this.InitializeComponent();

            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(SettingsViewModel<StorageFile, IRandomAccessStream>.StatusText):
#if NETFX_CORE
                    Bindings.Update();
#endif
                    break;
            }
        }
    }
}
