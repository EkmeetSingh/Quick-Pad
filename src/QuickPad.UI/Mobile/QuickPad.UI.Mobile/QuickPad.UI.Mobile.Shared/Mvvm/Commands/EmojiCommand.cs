using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement.Core;
#if NETFX_CORE
using Microsoft.AppCenter.Analytics;
#endif
using QuickPad.Mvvm.Commands;
using QuickPad.Mvvm.ViewModels;

namespace QuickPad.UI.Commands
{
    public class EmojiCommand : SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>, IEmojiCommand<StorageFile, IRandomAccessStream>, ICommand
    {
        public EmojiCommand()
        {
            Executioner = viewModel =>
            {
                viewModel.InvokeFocusTextBox();

                try //More error here
                {
                    CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
                }
                catch (Exception ex)
                {
#if NETFX_CORE
                    Analytics.TrackEvent($"Attempting to open emoji keyboard\r\n{ex.Message}");
#endif
                }

                return Task.CompletedTask;
            };
        }
    }
}