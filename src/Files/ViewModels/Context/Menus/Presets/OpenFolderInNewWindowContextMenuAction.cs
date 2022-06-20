using System;
using Files.Models.Actions;
using Files.Models.Messenger;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class OpenFolderInNewWindowContextMenuAction :
        StandardContextMenuItemViewModel
    {

        public OpenFolderInNewWindowContextMenuAction()
        {
            Command = new ExtendedRelayCommand(OnExecuteCommand, mayExecute: MayExecuteCommand);

            Header = "Open folder in new window";
        }
        
        public override bool MayExecute(BrowserContentViewModelBase? viewModel)
        {
            return viewModel?.Selection.SelectedItem is FileSystemItemViewModel item &&
                   item.IsFolder;
        }
        
        private bool MayExecuteCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return false;
            
            // ReSharper disable once MergeIntoPattern
            return parameter.SelectedItem is FileSystemItemViewModel vm &&
                   vm.IsFolder;
        }

        private void OnExecuteCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return;
            
            // ReSharper disable once MergeIntoPattern
            if (parameter.SelectedItem is not FileSystemItemViewModel vm)
                return;

            // ReSharper disable once MergeIntoPattern
            if (!vm.IsFolder)
                return;

            var request = new OpenFolderInNewWindowRequestModel
            {
                FolderUri = new Uri(vm.FullPath)
            };
            Messenger.Publish(request);
        }
    }
}