using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Files.Adb.Extensions;
using Files.Adb.Models;
using Files.Models.Actions;
using Files.ViewModels;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Android;
using Files.ViewModels.Context.Menus;
using Material.Icons;
using MinimalMvvm.ViewModels.Commands;

namespace Files.Services.Android.Actions
{
    public sealed class PullAdbContextMenuItemAction : StandardContextMenuItemViewModel
    {
        private const int OperationBufferSize = 4096;
        
        public PullAdbContextMenuItemAction()
        {
            Command = new RelayCommand(OnExecuteCommand);
            Icon = new MaterialIconViewModel(MaterialIconKind.Android);
            Header = "Pull";
        }

        private async void OnExecuteCommand(object? obj)
        {
            var app = Application.Current as FilesApp;

            var selectedItems = new List<AdbFileSystemItemViewModel>();

            switch (obj)
            {
                case BrowserActionParameterModel parameter:
                {
                    var items = new List<ItemViewModelBase>(parameter.SelectedItems ??
                                                            Array.Empty<ItemViewModelBase>());
                    
                    if(items.Count == 0 && parameter.SelectedItem is AdbFileSystemItemViewModel vm)
                        items.Add(vm);
                    
                    foreach (var item in items)
                    {
                        if (item is AdbFileSystemItemViewModel vm1)
                        {
                            selectedItems.Add(vm1);
                        }
                        else
                        {
                            throw new InvalidCastException("Some items are not AdbFileSystemItemViewModel");
                        }
                    }
                } break;
                
                case AdbFileSystemItemViewModel singleVm:
                    selectedItems.Add(singleVm);
                    break;
                
                case IEnumerable<AdbFileSystemItemViewModel> multipleVMs:
                    selectedItems.AddRange(multipleVMs);
                    break;
                
                default:
                    throw new Exception("Unknown type of object passed to AndroidCommandsBackend.OnExecutePullCommand");
            }

            var dialog = new OpenFolderDialog
            {
                Title = "Select destination folder"
            };

            var result = await dialog.ShowAsync(app!.WindowManager.LastFocusedWindow!);

            // User cancelled the operation
            if (result == null)
                return;

            var backend = AndroidDebugBackend.Instance;

            async Task Copy(AdbConnection conn, string remotePath, string subDir, string fileName)
            {
                var local = Path.Combine(result, subDir);
                var folder = Directory.CreateDirectory(local);
                var localFilePath = Path.Combine(folder.FullName, fileName);

                // TODO: Might exist file with same name already, should ask user how to do before continue execution.
                // ReSharper disable once ConvertToUsingDeclaration
                await using (var localStream = File.Create(localFilePath))
                {
                    await using (var stream = backend.GetFileStreamViaShell(conn, remotePath))
                    {
                        if (stream == null)
                            throw new Exception("Couldn't get file stream from remote device.");
                        
                        var buf = new byte[OperationBufferSize];
                        int len;

                        while ((len = await stream.ReadAsync(buf)) > 0)
                        {
                            localStream.Write(buf, 0, len);
                        }
                    }
                }
            }

            // TODO: Encapsulate procedure as operations 
            foreach (var item in selectedItems)
            {
                if (item.Parent is not AdbBrowserContentViewModel parentView)
                    throw new Exception("Procedure couldn't trust this element.");
                
                var uri = new Uri(item.FullPath);
                var connection = uri.ExtractAdbConnectionModelFromUri();
                var path = uri.LocalPath;

                // This one should work correctly I think
                var subPath = path.Remove(0, parentView.CurrentDirectory.Length);

                if (item.IsFolder)
                {
                    await foreach (var filePath in backend
                                       .WalkDirectoryTreeAsync(connection, path))
                    {
                        subPath = filePath.Remove(0, parentView.CurrentDirectory.Length);
                        var subFolder = Path.GetDirectoryName(subPath);
                        var fileName = Path.GetFileName(subPath);
                        await Copy(connection, filePath, subFolder ?? string.Empty, fileName);
                    }
                }
                else
                {
                    await Copy(connection, path, string.Empty, Path.GetFileName(subPath));
                }
            }
        }
    }
}