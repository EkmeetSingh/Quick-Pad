using System.Linq;
using System.Reflection;
using System.Windows.Input;
using QuickPad.Mvvm.Commands.Actions;
using QuickPad.Mvvm.Commands.Clipboard;
using QuickPad.Mvvm.Commands.Editing;
using QuickPad.Mvvm.ViewModels;


namespace QuickPad.Mvvm.Commands
{
    public class WindowsQuickPadCommands<TStorageFile, TStream> : IQuickPadCommands<TStorageFile, TStream>
        where TStream : class
    {
        public static void NotifyAll(DocumentViewModel<TStorageFile, TStream> viewModel,
            SettingsViewModel<TStorageFile, TStream> settings)
        {
            _commands.NotifyChanged(viewModel, settings);
        }

        public void RefreshStates(DocumentViewModel<TStorageFile, TStream> viewModel)
        {
            this.UndoCommand.InvokeCanExecuteChanged(viewModel);
            this.RedoCommand.InvokeCanExecuteChanged(viewModel);
        }

        private static WindowsQuickPadCommands<TStorageFile, TStream> _commands = null;

        public WindowsQuickPadCommands()
        {
        }

        public WindowsQuickPadCommands(
            IShowGoToCommand<TStorageFile, TStream> showGotoCommand
            , IShareCommand<TStorageFile, TStream> shareCommand
            , ICutCommand<TStorageFile, TStream> cutCommand
            , ICopyCommand<TStorageFile, TStream> copyCommand
            , IPasteCommand<TStorageFile, TStream> pasteCommand
            , IDeleteCommand<TStorageFile, TStream> deleteCommand
            , IContentChangedCommand<TStorageFile, TStream> contentChangedCommand
            , IEmojiCommand<TStorageFile, TStream> emojiCommand
            , ICompactOverlayCommand<TStorageFile, TStream> compactOverlayCommand
            , IRateAndReviewCommand<TStorageFile, TStream> rateAndReviewCommand)
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

        public IShareCommand<TStorageFile, TStream> ShareCommand { get; }
        public ICutCommand<TStorageFile, TStream> CutCommand { get; }
        public ICopyCommand<TStorageFile, TStream> CopyCommand { get; }
        public IPasteCommand<TStorageFile, TStream> PasteCommand { get; }
        public IDeleteCommand<TStorageFile, TStream> DeleteCommand { get; }
        public IContentChangedCommand<TStorageFile, TStream> ContentChangedCommand { get; }
        public IEmojiCommand<TStorageFile, TStream> EmojiCommand { get; }
        public ICompactOverlayCommand<TStorageFile, TStream> CompactOverlayCommand { get; }
        public IRateAndReviewCommand<TStorageFile, TStream> RateAndReviewCommand { get; }
        public IShowGoToCommand<TStorageFile, TStream> ShowGoToCommand { get; }

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SaveCommand { get; } =
            new SimpleCommand<DocumentViewModel<TStorageFile, TStream>>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SaveAsCommand { get; } =
            new SimpleCommand<DocumentViewModel<TStorageFile, TStream>>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> LoadCommand { get; } =
            new SimpleCommand<DocumentViewModel<TStorageFile, TStream>>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> NewDocumentCommand { get; } =
            new SimpleCommand<DocumentViewModel<TStorageFile, TStream>>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ExitCommand { get; } =
            new SimpleCommand<DocumentViewModel<TStorageFile, TStream>>();

        void IQuickPadCommands<TStorageFile, TStream>.NotifyAll(
            DocumentViewModel<TStorageFile, TStream> documentViewModel,
            SettingsViewModel<TStorageFile, TStream> settings)
        {
            NotifyAll(documentViewModel, settings);
        }

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> UndoCommand { get; } =
            new UndoCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> RedoCommand { get; } =
            new RedoCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> BingCommand { get; } =
            new SearchWithBingCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> GoogleCommand { get; } =
            new SearchWithGoogleCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ResetSettingsCommand { get; } =
            new ResetSettingsCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ImportSettingsCommand { get; } =
            new ImportSettingsCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ExportSettingsCommand { get; } =
            new ExportSettingsCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> HideFindCommand { get; } =
            new HideFindCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> FindNextCommand { get; } =
            new FindNextCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> FindPreviousCommand { get; } =
            new FindPreviousCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ReplaceNextCommand { get; } =
            new ReplaceNextCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ReplaceAllCommand { get; } =
            new ReplaceAllCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ZoomInCommand { get; } =
            new ZoomInCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ZoomOutCommand { get; } =
            new ZoomOutCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ResetZoomCommand { get; } =
            new ResetZoomCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SuperscriptCommand { get; } =
            new SuperscriptCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SubscriptCommand { get; } =
            new SubscriptCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> BoldCommand { get; } =
            new BoldCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ItalicsCommand { get; } =
            new ItalicCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> UnderlineCommand { get; } =
            new UnderlineCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> StrikeThroughCommand { get; } =
            new StrikeThroughCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> BulletsCommand { get; } =
            new BulletsCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> LeftAlignCommand { get; } =
            new LeftAlignCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> CenterAlignCommand { get; } =
            new CenterAlignCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> RightAlignCommand { get; } =
            new RightAlignCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> JustifyCommand { get; } =
            new JustifyCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ColorCommand { get; } =
            new ColorCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ToggleWordWrapCommand { get; } =
            new ToggleWordWrapCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> FocusCommand { get; } =
            new FocusCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> SettingsCommand { get; } =
            new ShowSettingsCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ShowCommandBarCommand { get; } =
            new ShowCommandBarCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ShowFontsCommand { get; } =
            new ShowFontsCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ShowMenusCommand { get; } =
            new ShowMenusCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ShowFindCommand { get; } =
            new ShowFindCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ShowReplaceCommand { get; } =
            new ShowReplaceCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SelectAllCommand { get; } =
            new SelectAllCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> InsertTimeDateCommand { get; } =
            new InsertTimeDateCommand<TStorageFile, TStream>();

        public SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> GoToLineCommand { get; } =
            new GoToLineCommand<TStorageFile, TStream>();

        public SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> AcknowledgeFontSelectionChangeCommand { get; } =
            new AcknowledgeFontSelectionChangeCommand<TStorageFile, TStream>();

        public void NotifyChanged(DocumentViewModel<TStorageFile, TStream> documentViewModel,
            SettingsViewModel<TStorageFile, TStream> settingsViewModel)
        {
            Enumerable.ToList(Enumerable.Where(GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public),
                pi =>
                    pi.PropertyType == typeof(SimpleCommand<DocumentViewModel<TStorageFile, TStream>>) ||
                    pi.PropertyType == typeof(SimpleCommand<SettingsViewModel<TStorageFile, TStream>>))).ForEach(pi =>
            {
                var documentCommand = pi.GetValue(this) as SimpleCommand<DocumentViewModel<TStorageFile, TStream>>;
                documentCommand?.InvokeCanExecuteChanged(documentViewModel);

                var settingsCommand = pi.GetValue(this) as SimpleCommand<SettingsViewModel<TStorageFile, TStream>>;
                settingsCommand?.InvokeCanExecuteChanged(settingsViewModel);
            });
        }
    }
}