using Avalonia.Input;
using Files.Models.Actions;
using Files.Services.Platform.Interfaces;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Local;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class ExecuteApplicationContextMenuAction :
        StandardContextMenuItemViewModel
    {
        public static ExecuteApplicationContextMenuAction? Instance => _instance;
        private static ExecuteApplicationContextMenuAction? _instance;

        private readonly IPlatformSupportExecuteApplication? _service;
        
        public ExecuteApplicationContextMenuAction(IPlatformSupportExecuteApplication service)
        {
            _instance = this;
            _service = service;

            Command = new ExtendedRelayCommand(OnExecuteApplicationCommand, CanExecute);

            Icon = MaterialIconViewModel.Launch;
            Shortcut = KeyGesture.Parse("Enter");
            Header = "Execute application";
        }

        private bool CanExecute(object? arg)
        {
            return MayExecuteProcedure(arg);
        }

        public override bool MayExecute(BrowserContentViewModelBase? vm)
        {
            return MayExecuteProcedure(vm?.Selection.SelectedItem);
        }

        private void OnExecuteApplicationCommand(object? arg)
        {
            if (arg is not BrowserActionParameterModel parameter)
                return;
            
            // ReSharper disable once MergeIntoPattern
            if(parameter.SelectedItem is not FileItemViewModel model)
                return;

            _service?.NativeExecuteApplication(model.FullPath);
        }

        private bool MayExecuteProcedure(object? item)
        {
            FileItemViewModel? file = null;
            
            switch (item)
            {
                case FileItemViewModel vm:
                {
                    file = vm;
                }
                    break;

                case BrowserActionParameterModel parameter:
                {
                    if(parameter.SelectedItem is FileItemViewModel viewModel)
                        file = viewModel;
                }
                    break;
            }
            
            return file is not null &&
                   (_service?.IsExecutableApplication(file.FullPath) ?? false);
        }
    }
}