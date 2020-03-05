using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using QuickPad.Mvvm.ViewModels;
using QuickPad.UI.Helpers;
using QuickPad.UI.Theme;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuickPad.UI.Controls.Settings
{
    public sealed partial class General : Page
    {
        public IVisualThemeSelector VtSelector => VisualThemeSelector.Current;
        public SettingsViewModel<StorageFile, IRandomAccessStream> Settings { get; } = App.Settings;
     
        public General()
        {
            this.InitializeComponent();
        }
    }
}
