using System;
using System.Threading;
using System.Threading.Tasks;
using Files.Adb.Extensions;
using Files.Adb.Models;
using Files.Services.Android;
using Files.ViewModels.Browser.Files.Android;

namespace Files.ViewModels.Browser
{
    public class AdbBrowserContentViewModel : BrowserContentViewModelBase
    {
        public AdbConnection? Connection { get; private set; }
        
        public string CurrentDirectory { get; private set; }

        public AdbBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {
            Connection = null;
            CurrentDirectory = string.Empty;
        }

        public override void LoadContent(Uri uri, CancellationToken cancellationToken = default)
        {
            EnumerateDirectoryAndFillCollection(uri, cancellationToken)
                .Wait(cancellationToken);
        }

        private async Task EnumerateDirectoryAndFillCollection(Uri uri, CancellationToken cancellationToken = default)
        {
            var adb = AndroidDebugBackend.Instance;

            var conn = uri.ExtractAdbConnectionModelFromUri();
            var dir = uri.LocalPath;

            Connection = conn;

            CurrentDirectory = dir;

            await foreach (var model in adb.SyncBackend?.GetListFilesViaSyncAsync(conn, dir)
                               .WithCancellation(cancellationToken)!)
            {
                if(model == null)
                    continue;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (model.IsDirectory)
                    {
                        AddItemOnUiThread(new AdbFolderItemViewModel(this, model));
                        break;
                    }
                    
                    AddItemOnUiThread(new AdbFileItemViewModel(this, model));
                    break;
                }
            }
        }
    }
}