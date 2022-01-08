using Avalonia.Controls;
using Files.Views.Models.Browser.Files.Local;

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
    }
}