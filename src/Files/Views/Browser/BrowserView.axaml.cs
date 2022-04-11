using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.LogicalTree;
using Files.Services;
using Files.ViewModels;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Local;
using Files.ViewModels.Browser.Preview;
using Files.ViewModels.Context.Menus;
using Files.Views.Controls;
using Files.Views.Controls.Events;

// ReSharper disable ConvertTypeCheckPatternToNullCheck
// ReSharper disable MergeIntoPattern

namespace Files.Views.Browser
{
    // ReSharper disable once UnusedType.Global
    public class BrowserView : ResourceDictionary
    {
        private SelectingItemRepeater _currentItems;
        private SelectingItemRepeater _prevItems;

        // ReSharper disable once UnusedMember.Local
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

            if (menu.DataContext is not ItemViewModelBase vm)
                return;
            
            var menus = ContextMenuBackend.Instance.GetContextMenu(vm);
            var parameter = GetSelectedItemOrItems(_currentItems.Selection);
            
            foreach (var model in menus)
            {
                model.CommandParameter = parameter;
            }
            
            menu.Items = menus;
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
            _prevItems = _currentItems;
            
            _currentItems = itemRepeater;

            if (itemRepeater.DataContext is not BrowserContentViewModelBase vm)
                return;
            
            UpdateSidesheetProperties(vm.Parent, e.AddedItems);
        }

        private void SelectingItemRepeater_OnAttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            if (sender is not SelectingItemRepeater itemRepeater)
                return;
            _prevItems = _currentItems;
            
            _currentItems = itemRepeater;
        }

        private void UpdateSidesheetProperties(BrowserWindowTabViewModel vm, IList? items)
        {
            if (!vm.IsSidesheetVisible)
                return;

            if (items is null)
                return;
            
            foreach (var item in items)
            {
                vm.ShowPropertiesSidesheet((item as ItemViewModelBase)!);
                break;
            }
        }
    }
}