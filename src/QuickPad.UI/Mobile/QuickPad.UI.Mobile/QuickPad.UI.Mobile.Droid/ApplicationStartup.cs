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
            services.AddSingleton<ApplicationController<StorageFile, IRandomAccessStream, AndroidDocumentManager>, ApplicationController<StorageFile, IRandomAccessStream, AndroidDocumentManager>>();
            services.AddSingleton(provider => App.Current as IApplication<StorageFile, IRandomAccessStream>);
            services.AddSingleton<SettingsViewModel<StorageFile, IRandomAccessStream>>(provider => provider.GetService<AndroidSettingsViewModel>());
            services.AddSingleton<AndroidSettingsViewModel, AndroidSettingsViewModel>();
            services.AddSingleton<AndroidSettingsModel, AndroidSettingsModel>();
            services.AddTransient<DocumentViewModel<StorageFile, IRandomAccessStream>, DocumentViewModel<StorageFile, IRandomAccessStream>>();
            services.AddSingleton<DefaultTextForegroundColor, DefaultTextForegroundColor>();
            services.AddSingleton<TextDocumentOptions, TextDocumentOptions>();
            services.AddTransient<IFindAndReplaceView<StorageFile, IRandomAccessStream>, FindAndReplaceViewModel<StorageFile, IRandomAccessStream>>();

            services.AddTransient(provider =>
            {
                var options = provider.GetService<TextDocumentOptions>();

                return new TextDocument(
                    options.Document
                    , options.Logger
                    , options.ViewModel
                    , provider.GetService<AndroidSettingsViewModel>()
                    , provider.GetService<IApplication<StorageFile, IRandomAccessStream>>());
            });

            services.AddTransient<DocumentModel<StorageFile, IRandomAccessStream>>(provider =>
            {
                var options = provider.GetService<TextDocumentOptions>();

                return new TextDocument(
                    options.Document
                    , options.Logger
                    , options.ViewModel
                    , provider.GetService<AndroidSettingsViewModel>()
                    , provider.GetService<IApplication<StorageFile, IRandomAccessStream>>());
            });
            services.AddSingleton<AndroidDocumentManager, AndroidDocumentManager>();
            services.AddSingleton<IDocumentViewModelStrings, DocumentViewModelStrings>();
            services.AddTransient(provider => ResourceLoader.GetForCurrentView());
            services.AddTransient(provider => App.RichEditBox?.Document);
                

            services.AddSingleton<IVisualThemeSelector, VisualThemeSelector>();

            services.AddSingleton<DialogManager, DialogManager>();
            services.AddTransient<AskToSave, AskToSave>();
            services.AddTransient<WelcomeDialog, WelcomeDialog>();
            services.AddTransient<AskForReviewDialog, AskForReviewDialog>();
            services.AddTransient<IGoToLineView<StorageFile, IRandomAccessStream>, GoToLine>();
            services.AddSingleton<MainPage, MainPage>();

            services.AddSingleton<IShowGoToCommand<StorageFile, IRandomAccessStream>, ShowGoToCommand<StorageFile, IRandomAccessStream>>();
            services.AddSingleton<IShareCommand<StorageFile, IRandomAccessStream>, ShareCommand>();
            services.AddSingleton<ICutCommand<StorageFile, IRandomAccessStream>, CutCommand>();
            services.AddSingleton<ICopyCommand<StorageFile, IRandomAccessStream>, CopyCommand>();
            services.AddSingleton<IPasteCommand<StorageFile, IRandomAccessStream>, PasteCommand>();
            services.AddSingleton<IDeleteCommand<StorageFile, IRandomAccessStream>, DeleteCommand>();
            services.AddSingleton<IContentChangedCommand<StorageFile, IRandomAccessStream>, ContentChangedCommand>();
            services.AddSingleton<IEmojiCommand<StorageFile, IRandomAccessStream>, EmojiCommand>();
            services.AddSingleton<IRateAndReviewCommand<StorageFile, IRandomAccessStream>, RateAndReviewCommand>();
            services.AddSingleton<AndroidQuickPadCommands, AndroidQuickPadCommands>();
            services.AddSingleton<IQuickPadCommands<StorageFile, IRandomAccessStream>, AndroidQuickPadCommands>();
            services.AddSingleton<PasteCommand, PasteCommand>();

            services.AddSingleton(_ => Application.Current as IApplication<StorageFile, IRandomAccessStream>);
            // Add additional services here.
        }

        public static void Configure(IConfigurationBuilder configuration)
        {
            // Add additional configuration here.
        }
    }
}
