using Avalonia.Input;
using Files.Models.Actions;
using Files.Services.Platform.Interfaces;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Local;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class OpenFileWithAppContextMenuAction :
        StandardContextMenuItemViewModel
    {
        private readonly IPlatformSupportShowOpenWithDialog? _service;
        
        public OpenFileWithAppContextMenuAction(IPlatformSupportShowOpenWithDialog? service)
        {
            _service = service;

            Command = new ExtendedRelayCommand(OnExecuteShowOpenWithDialogCommand);
            
            Icon = MaterialIconViewModel.Launch;
            Shortcut = KeyGesture.Parse("Enter");
            Header = "Open with ...";
        }

        public override bool MayExecute(BrowserContentViewModelBase? viewModel)
        {
            return MayExecuteProcedure(viewModel?.Selection.SelectedItem);
        }

        private void OnExecuteShowOpenWithDialogCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return;
            
            // ReSharper disable once MergeIntoPattern
            if(parameter.SelectedItem is not FileItemViewModel model)
                return;
            
            _service?.ShowOpenWithApplicationDialog(model.FullPath);
        }
        
        private bool MayExecuteProcedure(object? arg)
        {
            return arg is FileItemViewModel model &&
                   (_service?.CanHandleThisFile(model.FullPath) ?? false);
        }
    }
}