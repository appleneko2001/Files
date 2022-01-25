using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Files.Services;
using Files.Views.Controls;
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
        private SelectingItemRepeater _prevItemRepeater;

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

        private void BrowserViewContextMenu_OnDataContextChanged(object sender, EventArgs e)
        {
            if (sender is not ContextMenu menu)
                return;
            
            if (Application.Current is not Application app)
                return;

            switch (menu.DataContext)
            {
                case FileItemViewModel:
                {
                    var menus = app.Resources[ContextMenuBackend.FileContextMenuResourceName] as
                        ObservableCollection<ContextMenuItemViewModelBase>;
                    var parameter = GetSelectedItemOrItems(_prevItemRepeater.Selection);
                    
                    foreach (var model in menus)
                    {
                        model.CommandParameter = parameter;
                    }

                    menu.Items = menus;
                } break;
                case FolderItemViewModel:
                {
                    var menus = app.Resources[ContextMenuBackend.FolderContextMenuResourceName] as
                        ObservableCollection<ContextMenuItemViewModelBase>;
                    var parameter = GetSelectedItemOrItems(_prevItemRepeater.Selection);
                    
                    foreach (var model in menus)
                    {
                        model.CommandParameter = parameter;
                    }
                    
                    menu.Items = menus;
                } break;
                default:
                    menu.Items = menu.Items;
                    break;
            }
        }

        private object? GetSelectedItemOrItems(object value)
        {
            if (value is SelectionModel<object> selectionModel)
            {
                return selectionModel.Count switch
                {
                    1 => selectionModel.SelectedItem!,
                    > 1 => selectionModel.SelectedItems,
                    _ => null
                };
            }

            return null;
        }

        private void SelectingItemRepeater_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not SelectingItemRepeater itemRepeater)
                return;
            _prevItemRepeater = itemRepeater;
        }
    }
}