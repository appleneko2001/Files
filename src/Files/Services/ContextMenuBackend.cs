using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Files.Services.Platform.Interfaces;
using Files.ViewModels;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Local;
using Files.ViewModels.Context.Menus;
using Material.Icons;
using MinimalMvvm.ViewModels.Commands;
using ContextMenuItem = Files.ViewModels.Context.Menus.ContextMenuItemViewModel;

namespace Files.Services
{
    public class ContextMenuBackend
    {
        private static ContextMenuBackend _instance;
        public static ContextMenuBackend Instance => _instance;

        private FilesApp _appInstance;
        private CommandsBackend _commands;
        private PlatformHotkeyConfiguration? _hotkeyConfigs;

        private Collection<KeyValuePair<Type, 
            IEnumerable<ContextMenuItemViewModelBase>>> RegisteredContextMenus = new ();
        
        private IEnumerable<ContextMenuItemViewModelBase> _essentialContextMenuItems;

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        private ContextMenuBackend(FilesApp app)
        {
            if(_instance != null)
                throw new Exception("ContextMenuBackend already instantiated");
            
            _instance = this;
            
            _appInstance = app;
            _hotkeyConfigs = AvaloniaLocator.Current.GetService<PlatformHotkeyConfiguration>();

            if (_hotkeyConfigs == null)
                Console.WriteLine("Unable to get platform specified hotkey configurations.");
            //throw new ArgumentNullException(nameof(_hotkeyConfigs), "Unable to get platform specified hotkey configurations.");

            _commands = CommandsBackend.Instance;

            _essentialContextMenuItems = InitiatePostEssentialMenus();

            RegisterContextMenuItems<FileItemViewModel>(InitiateFileContextMenus());
            RegisterContextMenuItems<FolderItemViewModel>(InitiateFolderContextMenus());
        }

        public static void Initiate(FilesApp app)
        {
            _instance = new ContextMenuBackend(app);
        }

        public static bool RegisterContextMenuItems<T>(IEnumerable<ContextMenuItemViewModelBase> item)
            where T : ItemViewModelBase
        {
            var inst = Instance;
            try
            {
                var menus = new KeyValuePair<Type, IEnumerable<ContextMenuItemViewModelBase>>(
                    typeof(T), item);
                inst.RegisteredContextMenus.Add(menus);
                return inst.RegisteredContextMenus.Contains(menus);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private IEnumerable<ContextMenuItemViewModelBase> InitiateFileContextMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItem("Execute application", new MaterialIconViewModel(MaterialIconKind.Launch), _commands.ExecuteApplicationCommand, KeyGesture.Parse("Enter")),
                new ContextMenuItem("Open", new MaterialIconViewModel(MaterialIconKind.Launch), _commands.OpenFileViaPlatformCommand, KeyGesture.Parse("Enter")),
                new ContextMenuItem("Open with ...", new MaterialIconViewModel(MaterialIconKind.Launch), _commands.ShowOpenFileWithDialogCommand),
                
                ContextMenuItem.Separator
            };

            return list;
        }
        
        private IEnumerable<ContextMenuItemViewModelBase> InitiateFolderContextMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItem("Open folder", keyGesture: KeyGesture.Parse("Enter"), command: _commands.OpenFolderInCurrentViewCommand),
                //new ContextMenuItemViewModel("Open folder in new tab"),
                new ContextMenuItem("Open folder in new window", command: _commands.OpenFolderInNewWindowCommand)
            };

            if (_appInstance.PlatformApi is IPlatformSupportNativeExplorer featureSupport1)
            {
                list.Add(new ContextMenuItem($"Open folder with {featureSupport1.NativeExplorerName}", command: new ExtendedRelayCommand(
                    delegate(object o)
                    {
                        if(o is FolderItemViewModel folder)
                            featureSupport1.OpenFolderWithNativeExplorer(folder.FullPath);
                    }, mayExecute: o => o is FolderItemViewModel)));
            }
            
            list.Add(ContextMenuItem.Separator);

            return list;
        }
        
        private IEnumerable<ContextMenuItemViewModelBase> InitiatePostEssentialMenus()
        {
            // Not ready, yet
            // TODO: Complete the feature
            /*
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItemViewModel("Cut", new MaterialIconViewModel(MaterialIconKind.ContentCut),
                    keyGesture: GetFirstDefaultGesture(_hotkeyConfigs?.Cut)),
                new ContextMenuItemViewModel("Copy", new MaterialIconViewModel(MaterialIconKind.ContentCopy),
                    keyGesture: GetFirstDefaultGesture(_hotkeyConfigs?.Copy),
                    command: CommandsBackend.Instance.CopyItemsToClipboardCommand),
                new ContextMenuItemViewModel("Paste", new MaterialIconViewModel(MaterialIconKind.ContentPaste),
                    keyGesture: GetFirstDefaultGesture(_hotkeyConfigs?.Paste)),
                ContextMenuItem.Separator,
                new ContextMenuItemViewModel("Delete", new MaterialIconViewModel(MaterialIconKind.TrashCan),
                    keyGesture: KeyGesture.Parse("Delete"), command: _commands.DeleteItemsCommand),
                new ContextMenuItemViewModel("Rename", new MaterialIconViewModel(MaterialIconKind.Pencil)),
                ContextMenuItem.Separator,
                new ContextMenuItemViewModel("Properties", command: _commands.ShowPropertiesCommand)
            };

            return list;*/
            return new List<ContextMenuItemViewModelBase>();
        }
        
        public IEnumerable<ContextMenuItemViewModelBase> GetContextMenu(ItemViewModelBase item)
        {
            var list = new List<ContextMenuItemViewModelBase>();

            foreach (var (key, value) in RegisteredContextMenus)
            { 
                // Add items from the registered menus if the item is subclass or equal to the registered type
                if (key.IsInstanceOfType(item) || key == item.GetType())
                {
                    list.AddRange(value);
                }
            }

            list.AddRange(_essentialContextMenuItems);

            return list;
        }

        public IEnumerable<ContextMenuItemViewModelBase> GetContextMenu(BrowserContentViewModelBase content)
        {
            var list = new List<ContextMenuItemViewModelBase>();

            foreach (var pair in 
                     from pair in RegisteredContextMenus
                     let type = content.GetType()
                     where pair.Key == type
                     select pair)
            {
                list.AddRange(pair.Value);
            }
            
            list.AddRange(_essentialContextMenuItems);

            return list;
        }
        
        private KeyGesture? GetFirstDefaultGesture(List<KeyGesture>? keyGestures) =>
            keyGestures?.FirstOrDefault() ?? null;
    }
}