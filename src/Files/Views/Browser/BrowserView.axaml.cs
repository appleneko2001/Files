using Avalonia;
using Avalonia.Controls;
using Files.Views.Controls.Events;
using Files.Views.Models;
using Files.Views.Models.Browser.Files.Local;
using Files.Views.Models.Browser.Preview;

namespace Files.Views.Browser
{
    public class BrowserView : ResourceDictionary
    {
        private void OnSelectTemplateKeyForBrowserViewGridItem(object sender, SelectTemplateEventArgs e)
        {
            switch (e.DataContext)
            {
                case FileItemViewModel:
                    e.TemplateKey = "File";
                    break;
                case FolderItemViewModel:
                    e.TemplateKey = "Folder";
                    break;
            }
        }

        private void SelectingItemRepeater_OnDoubleTappedItemEvent(object sender, AdditionalEventArgs e)
        {
            if(e.Argument is not Visual v)
                return;

            if (v.DataContext is not ItemViewModelBase vm)
                return;

            if (vm.OnClickCommand == null)
                return;
            
            if(vm.OnClickCommand.CanExecute(vm))
                vm.OnClickCommand.Execute(vm);
        }

        private void BrowserViewPreviewElement_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            if (e.DataContext is PreviewableViewModelBase)
                e.TemplateKey = "Image";
        }
    }
}