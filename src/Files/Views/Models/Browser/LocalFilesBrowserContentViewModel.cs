using System;
using System.IO;
using System.Threading;
using Avalonia.Threading;
using Files.Views.Models.Browser.Files.Local;

namespace Files.Views.Models.Browser
{
    public class LocalFilesBrowserContentViewModel : BrowserContentViewModelBase
    {
        public LocalFilesBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {

        }

        public override void LoadContent(Uri uri)
        {
            var di = new DirectoryInfo(uri.AbsolutePath);
            if (!di.Exists)
                throw new DirectoryNotFoundException($"Directory \"{di.FullName}\" is invalid or not exists.");

            EnumerateDirectoryAndFillCollection(di);
        }

        public override void RequestPreviews(CancellationToken _cancellationToken = default)
        {
            foreach (var item in Content)
            {
                if(item is FileItemViewModel file)
                    file.TryGetPreview(_cancellationToken);
            }
        }

        private void EnumerateDirectoryAndFillCollection(DirectoryInfo di)
        {
            foreach (var directory in di.EnumerateDirectories())
            {
                AddItemOnUiThread(new FolderItemViewModel(this, directory));
            }

            foreach (var fileInfo in di.EnumerateFiles())
            {
                var file = new FileItemViewModel(this, fileInfo);
                
                AddItemOnUiThread(file);
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