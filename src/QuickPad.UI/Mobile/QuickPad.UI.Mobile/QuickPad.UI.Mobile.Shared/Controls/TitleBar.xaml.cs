using System.ComponentModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using QuickPad.Mvvm.ViewModels;
using QuickPad.Mvvm.Commands;
using QuickPad.UI.Helpers;
using QuickPad.UI.Theme;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuickPad.UI.Controls
{
    public sealed partial class TitleBar : UserControl
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

                value.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsDirtyMarker):
                    DirtyMarker.Text = ViewModel.IsDirtyMarker;
                    break;

                case nameof(ViewModel.Title):
                    Title.Text = ViewModel.Title;
                    break;
            }
        }

        public TitleBar()
        {
            this.InitializeComponent();
            Settings.PropertyChanged += Settings_PropertyChanged;
            Window.Current.SetTitleBar(trickyTitleBar);

            var flowDirectionSetting = "LTR";
            
            Settings.FlowDirection = flowDirectionSetting == "LTR" 
                ? Windows.UI.Xaml.FlowDirection.LeftToRight 
                : Windows.UI.Xaml.FlowDirection.RightToLeft;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.CompactOverlay):
                case nameof(Settings.TitleMargin):
#if NETFX_CORE
                    Bindings.Update();
#endif
                    break;

                case nameof(Settings.DefaultTextForegroundBrush):
                    var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                    titleBar.ForegroundColor = ((SolidColorBrush)Settings.DefaultTextForegroundBrush).Color;
                    titleBar.ButtonForegroundColor = ((SolidColorBrush)Settings.DefaultTextForegroundBrush).Color;
                    titleBar.ButtonHoverForegroundColor = ((SolidColorBrush)Settings.DefaultTextForegroundBrush).Color;
                    titleBar.ButtonPressedForegroundColor = ((SolidColorBrush)Settings.DefaultTextForegroundBrush).Color;

                    break;
            }
        }
    }
}
