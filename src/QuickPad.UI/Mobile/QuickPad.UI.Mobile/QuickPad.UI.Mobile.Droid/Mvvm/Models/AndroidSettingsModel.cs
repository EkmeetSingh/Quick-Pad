using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickPad.Mvvm;
using QuickPad.Mvvm.Models;
using QuickPad.Mvvm.ViewModels;
using QuickPad.UI.Theme;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;
#if NETFX_CORE
using Microsoft.Toolkit.Uwp.Helpers;
#endif

namespace QuickPad.UI.Helpers
{
    public class AndroidSettingsModel : SettingsModel<StorageFile, IRandomAccessStream>
    {
        private readonly IServiceProvider _serviceProvider;
        private string _defaultColor;

        public AndroidSettingsModel(ILogger<SettingsViewModel<StorageFile, IRandomAccessStream>> logger
            , IApplication<StorageFile, IRandomAccessStream> app
            , IServiceProvider serviceProvider) 
            : base(logger, app)
        {
            _serviceProvider = serviceProvider;
        }

        public void ResetSettings()
        {
        }

        public string DefaultTextForegroundColorString
        {
            get => Get(ColorHelper.ToHex(Colors.White));
            set => Set(value);
        }

        public string DefaultTextForegroundBrushString
        {
            get
            {
                _defaultColor ??= ColorHelper.ToHex(_serviceProvider.GetService<IVisualThemeSelector>().CurrentItem
                    .DefaultTextForegroundColor);

                return Get(_defaultColor);
            }
            set => Set(value);
        }

        public string DefaultLanguageString
        {
            get => Get("en-US");
            set => Set(value);
        }

        public string FlowDirection
        {
            get => Get(nameof(Windows.UI.Xaml.FlowDirection.LeftToRight));
            set => Set(value);
        }

        public override bool Set<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            propertyName = propertyName != null && propertyName.StartsWith("set_", StringComparison.InvariantCultureIgnoreCase)
                ? propertyName.Substring(4)
                : propertyName;

            TValue originalValue = default;
            
            return true;
        }

        public override TValue Get<TValue>(TValue defaultValue, [CallerMemberName] string propertyName = null)
        {
            var name = propertyName ??
                       throw new ArgumentNullException(nameof(propertyName), "Cannot store property of unnamed.");

            name = name.StartsWith("get_", StringComparison.InvariantCultureIgnoreCase)
                ? propertyName.Substring(4)
                : propertyName;

            Logger.LogDebug($"WindowsSettingsModel::Get<{typeof(TValue).Name}>({defaultValue}, {name});");

            return defaultValue;
        }
    }
}