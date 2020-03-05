using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickPad.Mvc;
using QuickPad.Mvvm;
using QuickPad.Mvvm.Commands;
using QuickPad.Mvvm.Commands.Actions;
using QuickPad.Mvvm.Managers;
using QuickPad.Mvvm.Models;
using QuickPad.Mvvm.ViewModels;
using QuickPad.Mvvm.Views;
using QuickPad.UI.Commands;
using QuickPad.UI.Commands.Clipboard;
using QuickPad.UI.Dialogs;
using QuickPad.UI.Helpers;
using QuickPad.UI.Theme;

namespace QuickPad.UI
{
    public class ApplicationStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // MVC and ViewModels
            ServiceCollectionServiceExtensions.AddSingleton<ApplicationController<StorageFile, IRandomAccessStream, WasmDocumentManager>, ApplicationController<StorageFile, IRandomAccessStream, WasmDocumentManager>>(services);
            ServiceCollectionServiceExtensions.AddSingleton(services, provider => App.Current as IApplication<StorageFile, IRandomAccessStream>);
            ServiceCollectionServiceExtensions.AddSingleton<SettingsViewModel<StorageFile, IRandomAccessStream>>(services, provider => ServiceProviderServiceExtensions.GetService<WasmSettingsViewModel>(provider));
            ServiceCollectionServiceExtensions.AddSingleton<WasmSettingsViewModel, WasmSettingsViewModel>(services);
            ServiceCollectionServiceExtensions.AddSingleton<WindowsSettingsModel, WindowsSettingsModel>(services);
            ServiceCollectionServiceExtensions.AddTransient<DocumentViewModel<StorageFile, IRandomAccessStream>, DocumentViewModel<StorageFile, IRandomAccessStream>>(services);
            ServiceCollectionServiceExtensions.AddSingleton<DefaultTextForegroundColor, DefaultTextForegroundColor>(services);
            ServiceCollectionServiceExtensions.AddSingleton<RtfDocumentOptions, RtfDocumentOptions>(services);
            ServiceCollectionServiceExtensions.AddSingleton<TextDocumentOptions, TextDocumentOptions>(services);
            ServiceCollectionServiceExtensions.AddTransient<IFindAndReplaceView<StorageFile, IRandomAccessStream>, FindAndReplaceViewModel<StorageFile, IRandomAccessStream>>(services);

            ServiceCollectionServiceExtensions.AddTransient(services, provider =>
            {
                var options = ServiceProviderServiceExtensions.GetService<RtfDocumentOptions>(provider);

                return new RtfDocument(
                    options.Document
                    , provider
                    , options.Logger
                    , options.ViewModel
                    , ServiceProviderServiceExtensions.GetService<WasmSettingsViewModel>(provider)
                    , ServiceProviderServiceExtensions.GetService<IApplication<StorageFile, IRandomAccessStream>>(provider));
            });

            ServiceCollectionServiceExtensions.AddTransient(services, provider =>
            {
                var options = ServiceProviderServiceExtensions.GetService<TextDocumentOptions>(provider);

                return new TextDocument(
                    options.Document
                    , options.Logger
                    , options.ViewModel
                    , ServiceProviderServiceExtensions.GetService<WasmSettingsViewModel>(provider)
                    , ServiceProviderServiceExtensions.GetService<IApplication<StorageFile, IRandomAccessStream>>(provider));
            });

            ServiceCollectionServiceExtensions.AddTransient<DocumentModel<StorageFile, IRandomAccessStream>>(services, provider =>
            {
                var vm = ServiceProviderServiceExtensions.GetService<DocumentViewModel<StorageFile, IRandomAccessStream>>(provider);

                if (vm != null)
                {
                    switch (vm.CurrentFileType.ToLowerInvariant().Trim('.'))
                    {
                        case "rtf":
                            return ServiceProviderServiceExtensions.GetService<RtfDocument>(provider);
                    }
                }

                return ServiceProviderServiceExtensions.GetService<TextDocument>(provider);
            });
            ServiceCollectionServiceExtensions.AddSingleton<WasmDocumentManager, WasmDocumentManager>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IDocumentViewModelStrings, DocumentViewModelStrings>(services);
            ServiceCollectionServiceExtensions.AddTransient(services, provider => ResourceLoader.GetForCurrentView());
            ServiceCollectionServiceExtensions.AddTransient(services, provider => App.RichEditBox?.Document);
                

            ServiceCollectionServiceExtensions.AddSingleton<IVisualThemeSelector, VisualThemeSelector>(services);

            ServiceCollectionServiceExtensions.AddSingleton<DialogManager, DialogManager>(services);
            ServiceCollectionServiceExtensions.AddTransient<AskToSave, AskToSave>(services);
            ServiceCollectionServiceExtensions.AddTransient<WelcomeDialog, WelcomeDialog>(services);
            ServiceCollectionServiceExtensions.AddTransient<AskForReviewDialog, AskForReviewDialog>(services);
            ServiceCollectionServiceExtensions.AddTransient<IGoToLineView<StorageFile, IRandomAccessStream>, GoToLine>(services);
            ServiceCollectionServiceExtensions.AddSingleton<MainPage, MainPage>(services);

            ServiceCollectionServiceExtensions.AddSingleton<IShowGoToCommand<StorageFile, IRandomAccessStream>, ShowGoToCommand<StorageFile, IRandomAccessStream>>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IShareCommand<StorageFile, IRandomAccessStream>, ShareCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<ICutCommand<StorageFile, IRandomAccessStream>, CutCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<ICopyCommand<StorageFile, IRandomAccessStream>, CopyCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IPasteCommand<StorageFile, IRandomAccessStream>, PasteCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IDeleteCommand<StorageFile, IRandomAccessStream>, DeleteCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IContentChangedCommand<StorageFile, IRandomAccessStream>, ContentChangedCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IEmojiCommand<StorageFile, IRandomAccessStream>, EmojiCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IRateAndReviewCommand<StorageFile, IRandomAccessStream>, RateAndReviewCommand>(services);
            ServiceCollectionServiceExtensions.AddSingleton<WasmQuickPadCommands<StorageFile, IRandomAccessStream>, WasmQuickPadCommands<StorageFile, IRandomAccessStream>>(services);
            ServiceCollectionServiceExtensions.AddSingleton<IQuickPadCommands<StorageFile, IRandomAccessStream>>(services, provider => ServiceProviderServiceExtensions.GetService<WasmQuickPadCommands<StorageFile, IRandomAccessStream>>(provider));
            ServiceCollectionServiceExtensions.AddSingleton<PasteCommand, PasteCommand>(services);

            ServiceCollectionServiceExtensions.AddSingleton(services, _ => Application.Current as IApplication<StorageFile, IRandomAccessStream>);
            // Add additional services here.
        }

        public static void Configure(IConfigurationBuilder configuration)
        {
            // Add additional configuration here.
        }
    }
}
