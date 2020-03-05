using QuickPad.Mvvm.ViewModels;

namespace QuickPad.Mvvm.Commands
{
    public interface IQuickPadCommands<TStorageFile, TStream>
        where TStream : class
    {
        IShareCommand<TStorageFile, TStream> ShareCommand { get; }
        ICutCommand<TStorageFile, TStream> CutCommand { get; }
        ICopyCommand<TStorageFile, TStream> CopyCommand { get; }
        IPasteCommand<TStorageFile, TStream> PasteCommand { get; }
        IDeleteCommand<TStorageFile, TStream> DeleteCommand { get; }
        IContentChangedCommand<TStorageFile, TStream> ContentChangedCommand { get; }
        IEmojiCommand<TStorageFile, TStream> EmojiCommand { get; }
        ICompactOverlayCommand<TStorageFile, TStream> CompactOverlayCommand { get; }
        IRateAndReviewCommand<TStorageFile, TStream> RateAndReviewCommand { get; }
        IShowGoToCommand<TStorageFile, TStream> ShowGoToCommand { get; }
      
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SaveCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> NewDocumentCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> LoadCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SaveAsCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ExitCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> UndoCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> RedoCommand { get; }
        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> BingCommand { get; }
        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> GoogleCommand { get; }
        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ResetSettingsCommand { get; }
        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ImportSettingsCommand { get; }
        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ExportSettingsCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> HideFindCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> FindNextCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> FindPreviousCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ReplaceNextCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ReplaceAllCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ZoomInCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ZoomOutCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ResetZoomCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SuperscriptCommand { get; }
        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SubscriptCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> BoldCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ItalicsCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> UnderlineCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> StrikeThroughCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> BulletsCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> LeftAlignCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> CenterAlignCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> RightAlignCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> JustifyCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ColorCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ToggleWordWrapCommand { get; }

        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> FocusCommand { get; }

        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> SettingsCommand { get; }

        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ShowCommandBarCommand { get; }

        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ShowFontsCommand { get; }

        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> ShowMenusCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ShowFindCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> ShowReplaceCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> SelectAllCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> InsertTimeDateCommand { get; }

        SimpleCommandBase<DocumentViewModel<TStorageFile, TStream>> GoToLineCommand { get; }

        SimpleCommandBase<SettingsViewModel<TStorageFile, TStream>> AcknowledgeFontSelectionChangeCommand { get; }

        void NotifyAll(DocumentViewModel<TStorageFile, TStream> documentViewModel,
            SettingsViewModel<TStorageFile, TStream> settings);

        void RefreshStates(DocumentViewModel<TStorageFile, TStream> viewModel);

        void NotifyChanged(DocumentViewModel<TStorageFile, TStream> documentViewModel,
            SettingsViewModel<TStorageFile, TStream> settingsViewModel);
    }
    public interface IRateAndReviewCommand<TStorageFile, TStream> : ISimpleCommand<SettingsViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface ICompactOverlayCommand<TStorageFile, TStream> : ISimpleCommand<SettingsViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface IEmojiCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface IContentChangedCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface IDeleteCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface ICopyCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface ICutCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface IShareCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface IShowGoToCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }

    public interface IPasteCommand<TStorageFile, TStream> : ISimpleCommand<DocumentViewModel<TStorageFile, TStream>>
        where TStream : class
    {
    }
}
