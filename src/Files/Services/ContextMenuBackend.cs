using System.Collections.Generic;
using Avalonia.Controls;
using Files.Views.Models;
using Files.Views.Models.Context.Menus;
using Material.Icons;

namespace Files.Services
{
    public class ContextMenuBackend
    {
        public const string FolderContextMenuResourceName = "FolderContextMenu";
        public const string FileContextMenuResourceName = "FileContextMenu";

        private static ContextMenuBackend _instance;
        public static ContextMenuBackend Instance => _instance;

        public static void Initiate(FilesApp app)
        {
            AddOrModifyResource(app.Resources, FileContextMenuResourceName,
                InitiateFileContextMenus());
            AddOrModifyResource(app.Resources, FolderContextMenuResourceName,
                InitiateFileContextMenus());
        }

        private static void AddOrModifyResource(IResourceDictionary dict, object key, object? value)
        {
            if (!dict.TryAdd(key, value))
                dict[key] = value;
        }

        private static IEnumerable<ContextMenuItemViewModelBase> InitiateFileContextMenus()
        {
            var list = new List<ContextMenuItemViewModelBase>();
            
            list.Add(new ContextMenuItemViewModel("Cut", new MaterialIconViewModel(MaterialIconKind.ContentCut)));
            list.Add(new ContextMenuItemViewModel("Copy", new MaterialIconViewModel(MaterialIconKind.ContentCopy)));
            list.Add(new ContextMenuItemViewModel("Paste", new MaterialIconViewModel(MaterialIconKind.ContentPaste)));
            list.Add(new ContextMenuItemViewModel("-"));
            list.Add(new ContextMenuItemViewModel("Properties"));

            return list;
        }
    }
}