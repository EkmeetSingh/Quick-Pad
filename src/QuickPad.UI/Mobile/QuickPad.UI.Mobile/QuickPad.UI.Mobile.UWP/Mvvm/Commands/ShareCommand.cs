using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using QuickPad.Mvvm.Commands;
using QuickPad.Mvvm.ViewModels;

namespace QuickPad.UI.Mobile.UWP.Mvvm.Commands
{
    public class WindowsShareCommand : SimpleCommand<DocumentViewModel<StorageFile, IRandomAccessStream>>, IShareCommand<StorageFile, IRandomAccessStream>, ICommand
    {
        public WindowsShareCommand()
        {
            Executioner = viewModel =>
            {
                DataTransferManager.ShowShareUI();
                return Task.CompletedTask;
            };
        }

    }
}