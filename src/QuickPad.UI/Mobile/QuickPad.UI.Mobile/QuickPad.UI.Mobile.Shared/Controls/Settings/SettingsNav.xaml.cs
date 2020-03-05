using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using QuickPad.Mvvm.ViewModels;
using QuickPad.UI.Theme;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuickPad.UI.Controls.Settings
{
    public sealed partial class SettingsNav 
    {
        public IVisualThemeSelector VtSelector => VisualThemeSelector.Current;

        public SettingsNav()
        {
            this.InitializeComponent();

            SettingsFrame.Navigate(typeof(General), new SuppressNavigationTransitionInfo());

            App.Settings.PropertyChanged += SettingsOnPropertyChanged;
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SettingsViewModel<StorageFile, IRandomAccessStream>.ShowSettingsTab):
                    settingNavView.SelectedItem = settingNavView.MenuItems[(int)App.Settings.ShowSettingsTab];
                    ShowTab(App.Settings.ShowSettingsTab);
                    break;
            }
        }

        private Type ShowTab(SettingsTabs settingsTab)
        {
            var pageType = settingsTab switch
            {
                SettingsTabs.General => typeof(General),
                SettingsTabs.Theme => typeof(Theme),
                SettingsTabs.Fonts => typeof(Font),
                SettingsTabs.Advanced => typeof(Advanced),
                SettingsTabs.About => typeof(About),
                _ => null
            };

            if (pageType != null)
            {
                SettingsFrame.Navigate(pageType, new SuppressNavigationTransitionInfo());
            }

            return pageType;
        }

        private void SettingNavView_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            GeneralItem.IsSelected = true;
            App.Settings.ShowSettings = false;
        }

        private void SettingNavView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            _ = args.InvokedItemContainer.Tag?.ToString() switch
            {
                "General" => ShowTab(SettingsTabs.General),
                "Theme" => ShowTab(SettingsTabs.Theme),
                "Font" => ShowTab(SettingsTabs.Fonts),
                "Advanced" => ShowTab(SettingsTabs.Advanced),
                "About" => ShowTab(SettingsTabs.About),
                _ => null
            };
        }
    }
}
