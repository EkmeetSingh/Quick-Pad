using Windows.ApplicationModel.Resources;
using QuickPad.Mvvm.ViewModels;

namespace QuickPad.UI.Helpers
{
    public class DocumentViewModelStrings : IDocumentViewModelStrings
    {
        public string RichTextDescription { get; }
        public string TextDescription { get; }
        public string Untitled { get; }

        public DocumentViewModelStrings(ResourceLoader resourceLoader)
        {
            RichTextDescription = resourceLoader.GetString("RichTextDescription");
            TextDescription = resourceLoader.GetString("TextDescription");
            Untitled = resourceLoader.GetString("Untitled");
        }
    }
}