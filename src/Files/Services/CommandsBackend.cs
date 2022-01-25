using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Files.Commands;
using Files.Services.Platform;
using Files.Views.Models;
using Files.Views.Models.Browser.Files.Local;
using Material.Dialog;
using Material.Dialog.Enums;

namespace Files.Services
{
    public class CommandsBackend 
    {
        private static CommandsBackend _instance;
        public static CommandsBackend Instance => _instance;

        private FilesApp _appInstance;
        private PlatformSpecificBridge? _nativeCommands;
        
        private CommandsBackend(FilesApp app)
        {
            _appInstance = app;
            app.ApplicationInitializationCompleted += OnApplicationInitializationCompletedHandler;
            
            ExecuteApplicationCommand = new(OnExecuteApplicationCommand, mayExecute: MayExecuteApplicationCommand);
            OpenFileViaPlatformCommand = new ExtendedRelayCommand(OnExecuteOpenFileViaPlatformCommand);
            ShowOpenFileWithDialogCommand = new ExtendedRelayCommand(OnExecuteShowOpenWithDialogCommand,
                mayExecute: MayExecuteShowOpenWithDialogCommand);

            OpenFolderInCurrentViewCommand = new ExtendedRelayCommand(OnExecuteOpenFolderInCurrentViewCommand, mayExecute: MayExecuteOpenFolderInCurrentViewCommand);
            OpenFolderInNewWindowCommand = new ExtendedRelayCommand(OnExecuteOpenFolderInNewWindowCommand, mayExecute: MayExecuteOpenFolderInNewWindowCommand);

            DeleteItemsCommand =
                new ExtendedRelayCommand(OnExecuteDeleteItemsCommand, mayExecute: MayDeleteItemsCommand);

            ShowPropertiesCommand = new ExtendedRelayCommand(OnExecuteShowPropertiesCommand,
                mayExecute: MayExecuteShowPropertiesCommand);
        }

        public static void Initiate(FilesApp app)
        {
            _instance = new CommandsBackend(app);
        }

        public static ICommand? GetPrimaryCommandForThisFile(FileItemViewModel model)
        {
            if (_instance.MayExecuteApplicationCommand(model))
                return _instance.ExecuteApplicationCommand;

            return _instance.OpenFileViaPlatformCommand;
        }
        
        private void OnApplicationInitializationCompletedHandler(object sender, EventArgs e)
        {
            if (sender is not FilesApp app)
                return;
            
            app.ApplicationInitializationCompleted -= OnApplicationInitializationCompletedHandler;
            app.ApplicationShutdown += OnApplicationShutdown;
                
            _nativeCommands = app.PlatformApi;
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            if (sender is not FilesApp app)
                return;
            
            app.ApplicationShutdown -= OnApplicationShutdown;
            _nativeCommands = null;
        }

        // Commands

        #region Execute application command

        public ExtendedRelayCommand ExecuteApplicationCommand { get; }

        private bool MayExecuteApplicationCommand(object arg)
        {
            return arg is FileItemViewModel model && _nativeCommands.IsExecutableApplication(model.FullPath);
        }

        private void OnExecuteApplicationCommand(object obj)
        {
            if(obj is not FileItemViewModel model)
                return;

            _nativeCommands.NativeExecuteApplication(model.FullPath);
        }

        #endregion


        #region Show "Open with..." dialog command

        public ExtendedRelayCommand ShowOpenFileWithDialogCommand { get; }
        
        private bool MayExecuteShowOpenWithDialogCommand(object arg)
        {
            return arg is FileItemViewModel model && !_nativeCommands.IsExecutableApplication(model.FullPath);
        }

        private void OnExecuteShowOpenWithDialogCommand(object obj)
        {
            if(obj is not FileItemViewModel model)
                return;
            
            _nativeCommands.ShowOpenWithApplicationDialog(model.FullPath);
        }

        #endregion
        
        public ExtendedRelayCommand OpenFileViaPlatformCommand { get; }
        
        private void OnExecuteOpenFileViaPlatformCommand(object obj)
        {
            if(obj is not FileItemViewModel model)
                return;
            
            _nativeCommands.LetPlatformHandleThisFile(model.FullPath);
        }
        
        
        // Commands for folder

        #region Open folder in current view command

        public ExtendedRelayCommand OpenFolderInCurrentViewCommand { get; }
        
        private bool MayExecuteOpenFolderInCurrentViewCommand(object arg)
        {
            return arg is FolderItemViewModel;
        }

        private void OnExecuteOpenFolderInCurrentViewCommand(object obj)
        {
            var command = FolderItemViewModel.OpenFolderCommand;
            
            if(command.CanExecute(obj))
                command.Execute(obj);
        }

        #endregion
        
        #region Open folder in new window command

        public ExtendedRelayCommand OpenFolderInNewWindowCommand { get; }
        
        private bool MayExecuteOpenFolderInNewWindowCommand(object arg)
        {
            return arg is FolderItemViewModel;
        }

        private void OnExecuteOpenFolderInNewWindowCommand(object obj)
        {
            if (obj is not FolderItemViewModel folder)
                return;
            
            var window = _appInstance.CreateBrowserWindow(new Uri(folder.FullPath));
            window.Show();
        }

        #endregion
        
        // Commands for any items (folders / files / etc.)
        
        #region Delete files or folders

        public ExtendedRelayCommand DeleteItemsCommand { get; }
        
        private bool MayDeleteItemsCommand(object arg)
        {
            switch (arg)
            {
                case ItemViewModelBase item:
                {
                    return !item.IsReadonly;
                } 

                case IEnumerable<ItemViewModelBase> items:
                {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var item in items)
                    {
                        if (!item.IsReadonly)
                            return true;
                    }

                    return false;
                }

                case IReadOnlyList<object> list:
                {
                    foreach (var obj in list)
                    {
                        if(obj is not ItemViewModelBase item)
                            continue;
                        
                        if (!item.IsReadonly)
                            return true;
                    }

                    return false;
                }
            }

            return false;
        }

        private async void OnExecuteDeleteItemsCommand(object args)
        {
            var list = new List<ItemViewModelBase>();

            if (args is IReadOnlyList<object> items)
            {
                foreach (var obj in items)
                {
                    if(obj is not ItemViewModelBase item)
                        continue;
                        
                    list.Add(item);
                }
            }
            else
            {
                if (args is not ItemViewModelBase item)
                    return;

                list.Add(item);
            }

            var textBuilder = new StringBuilder("You are about to delete next items:\n");

            var max = list.Count;
            if (max >= 5)
                max = 5;
            
            for (var i = 0; i < max; i++)
            {
                var item = list[i];

                textBuilder.AppendLine(item.DisplayName);

                if (i != max - 1)
                    continue;
                
                if(max < list.Count)
                    textBuilder.AppendLine($"and {(list.Count - max).ToString()} items.");
            }
            
            var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
            {
                WindowTitle = "Files: confirm action dialog",
                ContentHeader = "Confirm action",
                SupportingText = textBuilder.ToString(),
                DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.YesNo)
            });

            var result = await dialog.ShowDialog(_appInstance.WindowManager.LastFocusedWindow);
            if (result is null)
                return;

            if (result.GetResult == "yes")
            {
                // User confirmed action
                
                // TODO: Asynchronous delete files and folders task implement
            }
        }

        #endregion
        
        #region Show properties in sidesheel

        // Avalonia property
        public ExtendedRelayCommand ShowPropertiesCommand { get; }
        
        private bool MayExecuteShowPropertiesCommand(object arg)
        {
            return arg is FolderItemViewModel or FileItemViewModel;
        }

        private void OnExecuteShowPropertiesCommand(object obj)
        {
            switch (obj)
            {
                case ItemViewModelBase item:
                {
                    item.Parent.Parent.ShowPropertiesSidesheet(item);
                } break;
            }
        }

        #endregion
    }
}