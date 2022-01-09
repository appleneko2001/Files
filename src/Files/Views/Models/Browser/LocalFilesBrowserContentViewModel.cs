using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using Files.Views.Models.Browser.Files.Local;

namespace Files.Views.Models.Browser
{
    public class LocalFilesBrowserContentViewModel : BrowserContentViewModelBase
    {
        public LocalFilesBrowserContentViewModel(BrowserWindowTabViewModel parent, string path) : base(parent)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists)
                throw new DirectoryNotFoundException($"Directory \"{di.FullName}\" is invalid or not exists.");
            
            Task.Run(delegate
            {
                EnumerateDirectoryAndFillCollection(di);
            });
        }

        private void EnumerateDirectoryAndFillCollection(DirectoryInfo di)
        {
            foreach (var directory in di.EnumerateDirectories())
            {
                AddItemOnUiThread(new FolderItemViewModel(this, directory));
            }

            foreach (var files in di.EnumerateFiles())
            {
                AddItemOnUiThread(new FileItemViewModel(this, files));
            }
        }

        private void AddItem(ItemViewModelBase item) => Content.Add(item);

        private void AddItemOnUiThread(ItemViewModelBase item)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                AddItem(item);
            }, DispatcherPriority.Background).Wait();
        }
    }
}