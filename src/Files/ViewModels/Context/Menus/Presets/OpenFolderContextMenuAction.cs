using System;
using Avalonia.Input;
using Files.Models.Actions;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class OpenFolderContextMenuAction :
        StandardContextMenuItemViewModel
    {
        public static OpenFolderContextMenuAction Instance { get; } = new ();
        
        public OpenFolderContextMenuAction()
        {
            Command = new ExtendedRelayCommand(OnExecuteCommand,
                mayExecute: MayExecuteCommand);
            Header = "Open folder";
            Shortcut = KeyGesture.Parse("Enter");
        }

        public override bool MayExecute(BrowserContentViewModelBase? viewModel)
        {
            return viewModel?.Selection.SelectedItem is FileSystemItemViewModel item &&
                   item.IsFolder;
        }

        private static bool MayExecuteCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return false;

            var selectedItem = parameter.SelectedItem;

            // ReSharper disable once MergeIntoPattern
            return selectedItem is FileSystemItemViewModel vm &&
                   vm.IsFolder;
        }

        private static void OnExecuteCommand(object? arg)
        {
            FileSystemItemViewModel? item = null;
            
            switch (arg)
            {
                case BrowserActionParameterModel parameter:
                {
                    var selectedItem = parameter.SelectedItem;
                    
                    if (selectedItem is not FileSystemItemViewModel vm)
                        return;
            
                    item = vm;
                }
                    break;

                case FileSystemItemViewModel vm:
                {
                    item = vm;
                }
                    break;
            }

            if (item == null)
                return;
            
            if(item.IsFolder)
                item.Parent.Parent.Open(new Uri(item.FullPath));
        }
    }
}