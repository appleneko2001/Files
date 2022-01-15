using System;
using System.Collections;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Files.Services;
using Files.Views.Controls.Events;
using Files.Views.Models;
using Files.Views.Models.Browser.Files.Local;
using Files.Views.Models.Browser.Preview;
using Files.Views.Models.Context.Menus;

// ReSharper disable ConvertTypeCheckPatternToNullCheck
// ReSharper disable MergeIntoPattern

namespace Files.Views.Browser
{
    // ReSharper disable once UnusedType.Global
    public class BrowserView : ResourceDictionary
    {
        private void OnSelectTemplateKeyForBrowserViewGridItem(object sender, SelectTemplateEventArgs e)
        {
            e.TemplateKey = e.DataContext switch
            {
                FileItemViewModel => "File",
                FolderItemViewModel => "Folder",
                _ => e.TemplateKey
            };
        }

        private void SelectingItemRepeater_OnDoubleTappedItemEvent(object sender, AdditionalEventArgs e)
        {
            if (e.Argument is not Visual v)
                return;

            if (v.DataContext is not ItemViewModelBase vm)
                return;

            if (vm.OnClickCommand == null)
                return;

            if (vm.OnClickCommand.CanExecute(vm))
                vm.OnClickCommand.Execute(vm);
        }

        private void BrowserViewPreviewElement_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            if (e.DataContext is PreviewableViewModelBase)
                e.TemplateKey = "Image";
        }

        private void BrowserViewContextMenu_OnContextMenuOpening(object sender, CancelEventArgs e)
        {
            if (sender is not ContextMenu)
                e.Cancel = true;
        }

        private void BrowserViewContextMenuItemTemplate_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            e.TemplateKey = e.DataContext switch
            {
                ContextMenuItemViewModel => "Item",
                ContextMenuSeparatorViewModel => "Separator",
                _ => e.TemplateKey
            };
        }

        private void BrowserViewContextMenu_OnDataContextChanged(object sender, EventArgs e)
        {
            if (sender is not ContextMenu menu)
                return;
            
            if (Application.Current is not Application app)
                return;

            menu.Items = menu.DataContext switch
            {
                FileItemViewModel => app.Resources[ContextMenuBackend.FileContextMenuResourceName] as
                    IEnumerable,
                FolderItemViewModel => app.Resources[ContextMenuBackend.FolderContextMenuResourceName]
                    as IEnumerable,
                _ => menu.Items
            };
        }
    }
}