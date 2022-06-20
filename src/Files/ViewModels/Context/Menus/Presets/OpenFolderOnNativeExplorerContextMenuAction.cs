using System;
using Files.Models.Actions;
using Files.Services.Platform.Interfaces;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files;
using Files.ViewModels.Browser.Files.Local;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class OpenFolderOnNativeExplorerContextMenuAction 
        : StandardContextMenuItemViewModel
    {
        private readonly IPlatformSupportNativeExplorer _service;

        public OpenFolderOnNativeExplorerContextMenuAction(IPlatformSupportNativeExplorer service)
        {
            _service = service;

            Command = new ExtendedRelayCommand(OnExecuteCommand,
                CanExecuteCommand,
                CanExecuteCommand);
        }

        public override string? Header
        {
            get => $"Open folder on {_service.NativeExplorerName}";
            protected set => throw new NotSupportedException();
        }
        
        public override bool MayExecute(BrowserContentViewModelBase? viewModel)
        {
            return viewModel?.Selection.SelectedItem is FolderItemViewModel item &&
                   item.IsFolder && _service.CanOpenFolderWithNativeExplorer(item.FullPath);
        }

        private void OnExecuteCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return;
            
            if(parameter.SelectedItem is FolderItemViewModel folder)
                _service.OpenFolderWithNativeExplorer(folder.FullPath);
        }
        
        private bool CanExecuteCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return false;
            
            return parameter.SelectedItem is FolderItemViewModel folder && 
                   _service.CanOpenFolderWithNativeExplorer(folder.FullPath);
        }
    }
}