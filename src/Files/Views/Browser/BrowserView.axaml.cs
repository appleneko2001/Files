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

namespace Files.Views.Browser
{
    public class BrowserView : ResourceDictionary
    {
        private void OnSelectTemplateKeyForBrowserViewGridItem(object sender, SelectTemplateEventArgs e)
        {
            switch (e.DataContext)
            {
                case FileItemViewModel:
                    e.TemplateKey = "File";
                    break;
                case FolderItemViewModel:
                    e.TemplateKey = "Folder";
                    break;
            }
        }

        private void SelectingItemRepeater_OnDoubleTappedItemEvent(object sender, AdditionalEventArgs e)
        {
            if(e.Argument is not Visual v)
                return;

            if (v.DataContext is not ItemViewModelBase vm)
                return;

            if (vm.OnClickCommand == null)
                return;
            
            if(vm.OnClickCommand.CanExecute(vm))
                vm.OnClickCommand.Execute(vm);
        }

        private void BrowserViewPreviewElement_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            if (e.DataContext is PreviewableViewModelBase)
                e.TemplateKey = "Image";
        }

        private void BrowserViewContextMenu_OnContextMenuOpening(object sender, CancelEventArgs e)
        {
            if (sender is not ContextMenu menu)
            {
                e.Cancel = true;
                return;
            }
            
            switch (menu.DataContext)
            {
                case FileItemViewModel:
                {
                    menu.Items = Application.Current?.Resources[ContextMenuBackend.FileContextMenuResourceName] as IEnumerable;
                } break;
                
                case FolderItemViewModel:
                {
                    menu.Items = Application.Current?.Resources[ContextMenuBackend.FolderContextMenuResourceName] as IEnumerable;
                } break;
            }

            return;
        }

        private void BrowserViewContextMenuItemTemplate_OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            switch (e.DataContext)
            {
                case ContextMenuItemViewModel:
                    e.TemplateKey = "Item";
                    break;
                case ContextMenuSeparatorViewModel:
                    e.TemplateKey = "Separator";
                    break;
                
            }
        }

        private void BrowserViewItemFlyout_OnOpening(object sender, EventArgs e)
        {
            if (sender is not MenuFlyout flyout)
                return;
            if (flyout.Target == null)
                return;

            var dataContext = flyout.Target.DataContext;
            
            switch (dataContext)
            {
                case FileItemViewModel file:
                {
                    flyout.Items = Application.Current?.Resources[ContextMenuBackend.FileContextMenuResourceName] as IEnumerable;
                } break;
                
                case FolderItemViewModel folder:
                {
                    flyout.Items = Application.Current?.Resources[ContextMenuBackend.FolderContextMenuResourceName] as IEnumerable;
                } break;
            }
        }

        private void BrowserViewContextMenu_OnContextMenuClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }
    }
}