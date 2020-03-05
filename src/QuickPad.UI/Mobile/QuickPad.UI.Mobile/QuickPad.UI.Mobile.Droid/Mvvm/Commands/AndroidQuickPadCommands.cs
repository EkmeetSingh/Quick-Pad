using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using QuickPad.Mvvm.Commands.Actions;
using QuickPad.Mvvm.Commands.Clipboard;
using QuickPad.Mvvm.Commands.Editing;
using QuickPad.Mvvm.ViewModels;


namespace QuickPad.Mvvm.Commands
{   
    public class AndroidQuickPadCommands : IQuickPadCommands<StorageFile, IRandomAccessStream>
    {
        public static void NotifyAll(DocumentViewModel<StorageFile, IRandomAccessStream> viewModel, SettingsViewModel<StorageFile, IRandomAccessStream> settings)
        {
            _commands.NotifyChanged(viewModel, settings);
        }

        public void RefreshStates(DocumentViewModel<StorageFile, IRandomAccessStream> viewModel)
        {
            this.UndoCommand.InvokeCanExecuteChanged(viewModel);
            this.RedoCommand.InvokeCanExecuteChanged(viewModel);
        }

        private static AndroidQuickPadCommands _commands = null;

        public AndroidQuickPadCommands() { }
        public AndroidQuickPadCommands(
            IShowGoToCommand<StorageFile, IRandomAccessStream> showGotoCommand
            , IShareCommand<StorageFile, IRandomAccessStream> shareCommand
            , ICutCommand<StorageFile, IRandomAccessStream> cutCommand
            , ICopyCommand<StorageFile, IRandomAccessStream> copyCommand
            , IPasteCommand<StorageFile, IRandomAccessStream> pasteCommand
            , IDeleteCommand<StorageFile, IRandomAccessStream> deleteCommand
            , IContentChangedCommand<StorageFile, IRandomAccessStream> contentChangedCommand
            , IEmojiCommand<StorageFile, IRandomAccessStream> emojiCommand
            , ICompactOverlayCommand<StorageFile, IRandomAccessStream> compactOverlayCommand
            , IRateAndReviewCommand<StorageFile, IRandomAccessStream> rateAndReviewCommand)
        {
            _commands = this;

            ShareCommand = shareCommand;
            CutCommand = cutCommand;
            CopyCommand = copyCommand;
            PasteCommand = pasteCommand;
            DeleteCommand = deleteCommand;
            ContentChangedCommand = contentChangedCommand;
            EmojiCommand = emojiCommand;
            CompactOverlayCommand = compactOverlayCommand;
            RateAndReviewCommand = rateAndReviewCommand;
            ShowGoToCommand = showGotoCommand;
        }

        public IShareCommand<StorageFile, IRandomAccessStream> ShareCommand { get; }
        public ICutCommand<StorageFile, IRandomAccessStream> CutCommand { get; }
        public ICopyCommand<StorageFile, IRandomAccessStream> CopyCommand { get; }
        public IPasteCommand<StorageFile, IRandomAccessStream> PasteCommand { get; }
        public IDeleteCommand<StorageFile, IRandomAccessStream> DeleteCommand { get; }
        public IContentChangedCommand<StorageFile, IRandomAccessStream> ContentChangedCommand { get; }
        public IEmojiCommand<StorageFile, IRandomAccessStream> EmojiCommand { get; }
        public ICompactOverlayCommand<StorageFile, IRandomAccessStream> CompactOverlayCommand { get; }
        public IRateAndReviewCommand<StorageFile, IRandomAccessStream> RateAndReviewCommand { get; }
        public IShowGoToCommand<StorageFile, IRandomAccessStream> ShowGoToCommand { get; }

        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SaveCommand { get; } = new SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SaveAsCommand { get; } = new SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> LoadCommand { get; } = new SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> NewDocumentCommand { get; } = new SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ExitCommand { get; } = new SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>();

        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SaveCommandBase => SaveCommand;
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SaveAsCommandBase => SaveAsCommand;
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> LoadCommandBase => LoadCommand;
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> NewDocumentCommandBase => NewDocumentCommand;
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ExitCommandBase => ExitCommand;

        void IQuickPadCommands<StorageFile, IRandomAccessStream>.NotifyAll(DocumentViewModel<StorageFile, IRandomAccessStream> documentViewModel, SettingsViewModel<StorageFile, IRandomAccessStream> settings)
        {
            NotifyAll(documentViewModel, settings);
        }

        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> UndoCommand { get; } = new UndoCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> RedoCommand { get; } = new RedoCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> BingCommand { get; } = new SearchWithBingCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> GoogleCommand { get; } = new SearchWithGoogleCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> BoldCommand { get; } = new BoldCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ItalicsCommand { get; } = new ItalicCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> UnderlineCommand { get; } = new UnderlineCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> StrikeThroughCommand { get; } = new StrikeThroughCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> BulletsCommand { get; } = new BulletsCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> LeftAlignCommand { get; } = new LeftAlignCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> CenterAlignCommand { get; } = new CenterAlignCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> RightAlignCommand { get; } = new RightAlignCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> JustifyCommand { get; } = new JustifyCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ColorCommand { get; } = new ColorCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ToggleWordWrapCommand { get; } = new ToggleWordWrapCommand<StorageFile, IRandomAccessStream>(); 
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> FocusCommand { get; } = new FocusCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> SettingsCommand { get; } = new ShowSettingsCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> ShowCommandBarCommand { get; } = new ShowCommandBarCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> ShowFontsCommand { get; } = new ShowFontsCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> ShowMenusCommand { get; } = new ShowMenusCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> ResetSettingsCommand { get; } = new ResetSettingsCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> ImportSettingsCommand { get; } = new ImportSettingsCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> ExportSettingsCommand { get; } = new ExportSettingsCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ShowFindCommand { get; } = new ShowFindCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> HideFindCommand { get; } = new HideFindCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> FindNextCommand { get; } = new FindNextCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> FindPreviousCommand { get; } = new FindPreviousCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ShowReplaceCommand { get; } = new ShowReplaceCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ReplaceNextCommand { get; } = new ReplaceNextCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ReplaceAllCommand { get; } = new ReplaceAllCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SelectAllCommand { get; } = new SelectAllCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> InsertTimeDateCommand { get; } = new InsertTimeDateCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ZoomInCommand { get; } = new ZoomInCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ZoomOutCommand { get; } = new ZoomOutCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> ResetZoomCommand { get; } = new ResetZoomCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> GoToLineCommand { get; } = new GoToLineCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SuperscriptCommand { get; } = new SuperscriptCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<DocumentViewModel<StorageFile, IRandomAccessStream>> SubscriptCommand { get; } = new SubscriptCommand<StorageFile, IRandomAccessStream>();
        public SimpleCommandBase<SettingsViewModel<StorageFile, IRandomAccessStream>> AcknowledgeFontSelectionChangeCommand { get; } =
            new AcknowledgeFontSelectionChangeCommand<StorageFile, IRandomAccessStream>();

        public void NotifyChanged(DocumentViewModel<StorageFile, IRandomAccessStream> documentViewModel, SettingsViewModel<StorageFile, IRandomAccessStream> settingsViewModel)
        {
            GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(pi =>
                pi.PropertyType == typeof(SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>) ||
                pi.PropertyType == typeof(SimpleCommand<SettingsViewModel<StorageFile, IRandomAccessStream>>)).ToList().ForEach(pi =>
                {
                    var documentCommand = pi.GetValue(this) as SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>;
                    documentCommand?.InvokeCanExecuteChanged(documentViewModel);

                    var settingsCommand = pi.GetValue(this) as SimpleCommand<SettingsViewModel<StorageFile, IRandomAccessStream>>;
                    settingsCommand?.InvokeCanExecuteChanged(settingsViewModel);
                });
        }
    }

}