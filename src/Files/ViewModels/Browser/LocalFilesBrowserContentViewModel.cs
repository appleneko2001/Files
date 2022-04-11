using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;
using Avalonia.Threading;
using Files.ViewModels.Browser.Files.Local;

namespace Files.ViewModels.Browser
{
    public class LocalFilesBrowserContentViewModel : BrowserContentViewModelBase
    {
        public LocalFilesBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {

        }

        public override void LoadContent(Uri uri, CancellationToken _cancellationToken = default)
        {
            var di = new DirectoryInfo(HttpUtility.UrlDecode(uri.AbsolutePath));
            if (!di.Exists)
                throw new DirectoryNotFoundException($"Directory \"{di.FullName}\" is invalid or not exists.");

            EnumerateDirectoryAndFillCollection(di, _cancellationToken);
        }

        public override void RequestPreviews(CancellationToken _cancellationToken = default)
        {
            /*
            foreach (var item in Content)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                
                if(item is FileItemViewModel file)
                    file.TryGetPreview(_cancellationToken);
            }
            */
        }

        private void EnumerateDirectoryAndFillCollection(DirectoryInfo di, CancellationToken _cancellationToken = default)
        {
            const int maxBulkCount = 32;
            var bulkPool = new ArrayList(maxBulkCount);

            void Commit()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                
                AddItemsOnUiThread(bulkPool);
                bulkPool.Clear();
            }
            
            foreach (var directory in di.EnumerateDirectories())
            { 
                if (bulkPool.Count >= bulkPool.Capacity)
                {
                    Commit();
                }
                bulkPool.Add(new FolderItemViewModel(this, directory));
            }

            if (bulkPool.Count > 0) Commit();

            foreach (var fileInfo in di.EnumerateFiles())
            {
                var file = new FileItemViewModel(this, fileInfo);

                if (bulkPool.Count >= bulkPool.Capacity)
                {
                    Commit();
                }
                
                bulkPool.Add(file);
            }
            
            if (bulkPool.Count > 0)
            {
                Commit();
            }
        }
    }
}