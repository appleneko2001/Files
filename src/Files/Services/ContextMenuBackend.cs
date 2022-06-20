using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Files.Services.Platform.Interfaces;
using Files.ViewModels.Browser;
using Files.ViewModels.Context.Menus;
using Files.ViewModels.Context.Menus.Presets;
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

        private Collection<IReadOnlyList<ContextMenuItemViewModelBase>> RegisteredContextMenus = new ();
        
        private readonly IEnumerable<ContextMenuItemViewModelBase> _essentialContextMenuItems;

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        private ContextMenuBackend(FilesApp app)
        {
            if(_instance != null)
                throw new Exception("ContextMenuBackend already instantiated");
            
            _instance = this;

            //_appInstance = app;
            _hotkeyConfigs = AvaloniaLocator.Current.GetService<PlatformHotkeyConfiguration>();

            if (_hotkeyConfigs == null)
                Console.WriteLine("Unable to get platform specified hotkey configurations.");
            //throw new ArgumentNullException(nameof(_hotkeyConfigs), "Unable to get platform specified hotkey configurations.");

            _commands = CommandsBackend.Instance;

            _essentialContextMenuItems = InitiatePostEssentialMenus();

            RegisterContextMenuItems(InitiatePreContextMenus());
        }

        public static void Initiate(FilesApp app)
        {
            _instance = new ContextMenuBackend(app);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static bool RegisterContextMenuItems(IReadOnlyList<ContextMenuItemViewModelBase> items)
        {
            var inst = Instance;
            try
            {
                inst.RegisteredContextMenus.Add(items);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private IReadOnlyList<ContextMenuItemViewModelBase> InitiatePreContextMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>();
            
            var execAppService = AvaloniaLocator
                .Current
                .GetService<IPlatformSupportExecuteApplication>();

            if(execAppService != null)
                list.Add(new ExecuteApplicationContextMenuAction(execAppService));
            
            var openFilePrimaryActionService = AvaloniaLocator
                .Current
                .GetService<IPlatformSupportOpenFilePrimaryAction>();
            
            if(openFilePrimaryActionService != null)
                list.Add(new OpenFileContextMenuAction(openFilePrimaryActionService));

            var openFileWithService = AvaloniaLocator
                .Current
                .GetService<IPlatformSupportShowOpenWithDialog>();
            
            if(openFileWithService != null)
                list.Add(new OpenFileWithAppContextMenuAction(openFileWithService));
            
            list.AddRange(new ContextMenuItemViewModelBase[]
            {
                OpenFolderContextMenuAction.Instance,
                new OpenFolderInNewTabContextMenuAction(),
                new OpenFolderInNewWindowContextMenuAction()
            });
            
            // Platform feature: Open folder on platform specific default app
            var platformFeature1 = AvaloniaLocator
                .Current
                .GetService<IPlatformSupportNativeExplorer>();

            if (platformFeature1 is not null)
            {
                list.Add(new OpenFolderOnNativeExplorerContextMenuAction(platformFeature1));
            }

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
            return new List<ContextMenuItemViewModelBase>
            {
                new DummyContextMenuItemViewModel()
            };
        }
        
        /*
        public IEnumerable<ContextMenuItemViewModelBase> GetContextMenu(ItemViewModelBase item, object? param)
        {
            var list = new List<ContextMenuItemViewModelBase>();

            foreach (var item in RegisteredContextMenus)
            {
                // Add items from the registered menus if the item is subclass or equal to the registered type
                if (!key.IsInstanceOfType(item) && key != item.GetType())
                    continue;
                
                foreach (var menu in value)
                {
                    if (menu.CommandParameter != ContextMenuItemViewModel.Separator)
                        menu.CommandParameter = param;
                        
                    list.Add(menu);
                }
            }

            list.AddRange(_essentialContextMenuItems);

            return list;
        }
        */

        public IEnumerable<ContextMenuItemViewModelBase> GetContextMenu(BrowserContentViewModelBase content)
        {
            var list = new List<ContextMenuItemViewModelBase>();

            foreach (var menus in RegisteredContextMenus)
            {
                var items = menus
                    .Where(e => e.MayExecute(content))
                    .ToList();
                
                if(!items.Any())
                    continue;
                
                list.AddRange(items);
                list.Add(ContextMenuItemViewModel.Separator);
            }

            list.AddRange(_essentialContextMenuItems);

            return list;
        }
        
        private KeyGesture? GetFirstDefaultGesture(List<KeyGesture>? keyGestures) =>
            keyGestures?.FirstOrDefault() ?? null;
    }
}