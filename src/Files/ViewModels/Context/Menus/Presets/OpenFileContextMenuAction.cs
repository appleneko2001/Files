using Avalonia.Input;
using Files.Models.Actions;
using Files.Services.Platform.Interfaces;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Local;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class OpenFileContextMenuAction :
        StandardContextMenuItemViewModel
    {
        public static OpenFileContextMenuAction? Instance => _instance;
        private static OpenFileContextMenuAction? _instance;
        
        private readonly IPlatformSupportOpenFilePrimaryAction? _service;
        
        public OpenFileContextMenuAction(IPlatformSupportOpenFilePrimaryAction? service)
        {
            _instance = this;
            _service = service;

            Command = new ExtendedRelayCommand(OnExecuteOpenFileViaPlatformCommand);
            
            Icon = MaterialIconViewModel.Launch;
            Shortcut = KeyGesture.Parse("Enter");
            Header = "Open";
        }

        public override bool MayExecute(BrowserContentViewModelBase? viewModel)
        {
            return MayExecuteProcedure(viewModel?.Selection.SelectedItem);
        }
        
        private void OnExecuteOpenFileViaPlatformCommand(object? arg)
        {
            FileItemViewModel? file = null;
            
            switch (arg)
            {
                case BrowserActionParameterModel parameter:
                {
                    if (parameter.SelectedItem is FileItemViewModel vm)
                        file = vm;
                }
                    break;
                
                case FileItemViewModel vm:
                    file = vm;
                    break;
            }

            if(file is null)
                return;
            
            _service?.LetPlatformHandleThisFile(file.FullPath);
        }

        private bool MayExecuteProcedure(object? arg)
        {
            return arg is FileItemViewModel model &&
                   (_service?.CanHandleThisFile(model.FullPath) ?? false);
        }
    }
}