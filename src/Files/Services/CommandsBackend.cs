using System;
using System.Windows.Input;
using Files.Commands;
using Files.Views.Models.Browser.Files.Local;

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
    }
}