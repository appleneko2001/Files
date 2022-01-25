using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Files.Commands;
using Files.Services.Platform.Interfaces;
using Files.Views.Models;
using Files.Views.Models.Browser.Files.Local;
using Files.Views.Models.Context.Menus;
using Material.Icons;

using ContextMenuItem = Files.Views.Models.Context.Menus.ContextMenuItemViewModel;

namespace Files.Services
{
    public class ContextMenuBackend
    {
        public const string FolderContextMenuResourceName = "FolderContextMenu";
        public const string FileContextMenuResourceName = "FileContextMenu";

        private static ContextMenuBackend _instance;
        public static ContextMenuBackend Instance => _instance;

        private FilesApp _appInstance;
        private CommandsBackend _commands;
        private PlatformHotkeyConfiguration? _hotkeyConfigs;

        private ObservableCollection<ContextMenuItemViewModelBase> FolderContextMenus;
        private ObservableCollection<ContextMenuItemViewModelBase> FileContextMenus;

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        private ContextMenuBackend(FilesApp app)
        {
            _hotkeyConfigs = AvaloniaLocator.Current.GetService<PlatformHotkeyConfiguration>();

            if (_hotkeyConfigs == null)
                Console.WriteLine("Unable to get platform specified hotkey configurations.");
            //throw new ArgumentNullException(nameof(_hotkeyConfigs), "Unable to get platform specified hotkey configurations.");

            _commands = CommandsBackend.Instance;

            FolderContextMenus = new ObservableCollection<ContextMenuItemViewModelBase>(InitiateFolderContextMenus());
            FileContextMenus = new ObservableCollection<ContextMenuItemViewModelBase>(InitiateFileContextMenus());
            
            AddOrModifyResource(app.Resources, FolderContextMenuResourceName, FolderContextMenus);
            AddOrModifyResource(app.Resources, FileContextMenuResourceName, FileContextMenus);
        }

        public static void Initiate(FilesApp app)
        {
            _instance = new ContextMenuBackend(app);
        }

        private static void AddOrModifyResource(IResourceDictionary dict, object key, object? value)
        {
            if (!dict.TryAdd(key, value))
                dict[key] = value;
        }

        private IEnumerable<ContextMenuItemViewModelBase> InitiateFileContextMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItem("Execute application", new MaterialIconViewModel(MaterialIconKind.Launch), _commands.ExecuteApplicationCommand, KeyGesture.Parse("Enter")),
                new ContextMenuItem("Open", new MaterialIconViewModel(MaterialIconKind.Launch), _commands.OpenFileViaPlatformCommand, KeyGesture.Parse("Enter")),
                new ContextMenuItem("Open with ...", new MaterialIconViewModel(MaterialIconKind.Launch), _commands.ShowOpenFileWithDialogCommand),
                
                ContextMenuItem.Separator,
            };

            list.AddRange(InitiatePostEssentialMenus());

            return list;
        }
        
        private IEnumerable<ContextMenuItemViewModelBase> InitiateFolderContextMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItem("Open folder", keyGesture: KeyGesture.Parse("Enter"), command: _commands.OpenFolderInCurrentViewCommand),
                //new ContextMenuItemViewModel("Open folder in new tab"),
                new ContextMenuItemViewModel("Open folder in new window", command: _commands.OpenFolderInNewWindowCommand),
            };

            if (_appInstance.PlatformApi is IPlatformSupportNativeExplorer featureSupport1)
            {
                list.Add(new ContextMenuItemViewModel($"Open folder with {featureSupport1.NativeExplorerName}", command: new ExtendedRelayCommand(
                    delegate(object o)
                    {
                        if(o is FolderItemViewModel folder)
                            featureSupport1.OpenFolderWithNativeExplorer(folder.FullPath);
                    }, mayExecute: delegate(object o)
                    {
                        return o is FolderItemViewModel;
                    })));
            }
            
            list.Add(ContextMenuItem.Separator);

            list.AddRange(InitiatePostEssentialMenus());

            return list;
        }
        
        private IEnumerable<ContextMenuItemViewModelBase> InitiatePostEssentialMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItemViewModel("Cut", new MaterialIconViewModel(MaterialIconKind.ContentCut),
                    keyGesture: GetFirstDefaultGesture(_hotkeyConfigs?.Cut)),
                new ContextMenuItemViewModel("Copy", new MaterialIconViewModel(MaterialIconKind.ContentCopy),
                    keyGesture: GetFirstDefaultGesture(_hotkeyConfigs?.Copy)),
                new ContextMenuItemViewModel("Paste", new MaterialIconViewModel(MaterialIconKind.ContentPaste),
                    keyGesture: GetFirstDefaultGesture(_hotkeyConfigs?.Paste)),
                ContextMenuItem.Separator,
                new ContextMenuItemViewModel("Delete", new MaterialIconViewModel(MaterialIconKind.TrashCan),
                    keyGesture: KeyGesture.Parse("Delete"), command: _commands.DeleteItemsCommand),
                new ContextMenuItemViewModel("Rename", new MaterialIconViewModel(MaterialIconKind.Pencil)),
                ContextMenuItem.Separator,
                new ContextMenuItemViewModel("Properties", command: _commands.ShowPropertiesCommand)
            };

            return list;
        }

        private KeyGesture? GetFirstDefaultGesture(List<KeyGesture>? keyGestures) =>
            keyGestures?.FirstOrDefault() ?? null;
    }
}