using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Input;
using Files.ViewModels;
using Files.ViewModels.Browser.Files;
using Files.ViewModels.Browser.Files.Local;
using Material.Dialog;
using Material.Dialog.Enums;
using MinimalMvvm.ViewModels.Commands;

namespace Files.Services
{
    public class CommandsBackend 
    {
        private static CommandsBackend _instance;
        public static CommandsBackend Instance => _instance;

        private FilesApp _appInstance;
        
        private CommandsBackend(FilesApp app)
        {
            _appInstance = app;

            CopyItemsToClipboardCommand = new ExtendedRelayCommand(OnExecuteCopyItemsToClipboardCommand);
            
            OpenFolderInNewWindowCommand = new ExtendedRelayCommand(OnExecuteOpenFolderInNewWindowCommand, mayExecute: MayExecuteOpenFolderInNewWindowCommand);

            DeleteItemsCommand =
                new ExtendedRelayCommand(OnExecuteDeleteItemsCommand, mayExecute: MayDeleteItemsCommand);

            ShowPropertiesCommand = new ExtendedRelayCommand(OnExecuteShowPropertiesCommand,
                mayExecute: MayExecuteShowPropertiesCommand);
        }

        // Commands
        
        // Copy selected files and folders path to clipboard
        
        public ExtendedRelayCommand CopyItemsToClipboardCommand { get; }

        private void OnExecuteCopyItemsToClipboardCommand(object arg)
        {
            var dataObject = new DataObject();
            
            switch (arg)
            {
                case FileSystemItemViewModel singleVm:
                    dataObject.Set("SelectedItems", 
                        new List<object>{singleVm}.ToImmutableList());
                    break;
                
                case IReadOnlyList<object> multiSelectVm:
                    if (multiSelectVm.Any(item => item is not FileSystemItemViewModel))
                        throw new InvalidCastException();

                    dataObject.Set("SelectedItems", multiSelectVm);
                    break;
                
                default:
                    return;
            }
            
            dataObject.Set("Operation", "Copy");
            
            Application.Current!.Clipboard?.SetDataObjectAsync(dataObject);
        }
        
        
        // Commands for folder
        
        
        #region Open folder in new window command

        public ExtendedRelayCommand OpenFolderInNewWindowCommand { get; }
        
        private bool MayExecuteOpenFolderInNewWindowCommand(object arg)
        {
            return arg is FileSystemItemViewModel vm &&
                   vm.IsFolder;
        }

        private void OnExecuteOpenFolderInNewWindowCommand(object obj)
        {
            if (obj is not FileSystemItemViewModel vm)
                return;

            if (!vm.IsFolder)
                return;
            
            var window = _appInstance.CreateBrowserWindow(new Uri(vm.FullPath));
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
        
        // TODO: Properties interface implement
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