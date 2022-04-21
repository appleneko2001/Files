using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Files.ViewModels.Browser.Files.Android;
using MinimalMvvm.ViewModels.Commands;

namespace Files.Services.Android
{
    public class AndroidCommandsBackend
    {
        private static ICommand _pullCommand = new RelayCommand(OnExecutePullCommand);

        private static async void OnExecutePullCommand(object? obj)
        {
            var app = Application.Current as FilesApp;

            var selectedItems = new List<AdbFileSystemItemViewModel>();

            switch (obj)
            {
                case AdbFileSystemItemViewModel singleVM:
                    selectedItems.Add(singleVM);
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

            var result = await dialog.ShowAsync(app.WindowManager.LastFocusedWindow);

            // User cancelled the operation
            if (result == null)
                return;
            
            // TODO: Complete feature implementation
        }

        public static ICommand PullCommand => _pullCommand;
    }
}