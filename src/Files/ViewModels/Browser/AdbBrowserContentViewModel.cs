using System;
using System.Threading;
using System.Threading.Tasks;
using Files.Adb.Models.Connections;
using Files.Models.Android.Enums;
using Files.Services.Android;
using Files.ViewModels.Browser.Files.Android;

namespace Files.ViewModels.Browser
{
    public class AdbBrowserContentViewModel : BrowserContentViewModelBase
    {
        public IAdbConnection Connection { get; private set; }

        public AdbBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {
        }

        public override void LoadContent(Uri uri, CancellationToken _cancellationToken = default)
        {
            EnumerateDirectoryAndFillCollection(uri, _cancellationToken)
                .Wait(_cancellationToken);
        }

        private async Task EnumerateDirectoryAndFillCollection(Uri uri, CancellationToken _cancellationToken = default)
        {
            IAdbConnection Parse(Uri uri)
            {
                if (uri.Host.Contains(':'))
                    return new AdbWifiConnection(uri.Host, uri.Port);
                return new AdbNormalConnection(uri.Host);
            }

            var adb = AndroidDebugBackend.Instance;

            var conn = Parse(uri);
            var dir = uri.LocalPath;

            Connection = conn;

            await foreach (var model in adb.GetListFilesAsync(conn, dir)
                               .WithCancellation(_cancellationToken))
            {
                if(model == null)
                    continue;

                var linkTarget = model.LinkTarget;

                while (true)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    
                    var isFile = model.Kind.HasFlag(LinuxFileSystemEntryKind.File);
                    var isDir = model.Kind.HasFlag(LinuxFileSystemEntryKind.Directory);

                    /* if (model.IsSymlink && !(isDir || isFile))
                    {
                        await foreach (var e in adb.GetListFilesAsync(conn, linkTarget!)
                                           .WithCancellation(_cancellationToken))
                        {
                            model.Kind |= linkTargetModel.Kind;
                            linkTarget = linkTargetModel.LinkTarget;
                        }

                        continue;
                    }*/

                    if (isDir)
                    {
                        AddItemOnUiThread(new AdbFolderItemViewModel(this, model));
                    }
                    else if (isFile)
                    {
                        AddItemOnUiThread(new AdbFileItemViewModel(this, model));
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        public override void RequestPreviews(CancellationToken _cancellationToken = default)
        {
            // TODO: Implement this feature
        }
    }
}