using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Files.Models.Actions;
using Files.Services;
using Files.ViewModels;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Preview;
using Files.Views.Controls;
using Files.Views.Controls.Events;
using Material.Styles.Controls;

// ReSharper disable ConvertTypeCheckPatternToNullCheck
// ReSharper disable MergeIntoPattern

namespace Files.Views.Browser
{
    // ReSharper disable once UnusedType.Global
    public class BrowserView : ResourceDictionary
    {
        private CustomItemRepeater _currentItems;

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

        private void SetContextMenu(ContextMenu menu,
            BrowserContentViewModelBase content)
        {
            var parameter = new BrowserActionParameterModel
            {
                BrowserViewModel = content
            };

            if (content.Selection.SelectedItem is ItemViewModelBase selectedItem)
            {
                parameter.SelectedItem = selectedItem;
            }
            
            if (content.Selection.SelectedItems is IReadOnlyList<ItemViewModelBase> selectedItems)
            {
                parameter.SelectedItems = selectedItems;
            }
            
            var menus = ContextMenuBackend.Instance.GetContextMenu(content);
            
            foreach (var item in menus)
            {
                item.CommandParameter = parameter;
            }
            menu.Items = ContextMenuBackend.Instance.GetContextMenu(content);
        }
        
        private void BrowserViewContextMenu_OnMenuOpened(object sender, RoutedEventArgs e)
        {
            if (sender is not ContextMenu menu)
                return;

            if (menu.DataContext is not BrowserContentViewModelBase contentVm)
                return;
            
            SetContextMenu(menu, contentVm);
        }
        
        private void BrowserViewContextMenu_OnMenuClosed(object sender, RoutedEventArgs e)
        {
            if (sender is not ContextMenu menu)
                return;

            menu.Items = null;
        }

        /*
        private void SetContextMenuOnSelectedItems(ContextMenu menu, ItemViewModelBase vm)
        {
            var parameter = GetSelectedItemOrItems(_currentItems.Selection);
            menu.Items = ContextMenuBackend.Instance.GetContextMenu(vm, parameter);
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
        */

        private void SelectingItemRepeater_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not CustomItemRepeater itemRepeater)
                return;
            
            _currentItems = itemRepeater;

            if (itemRepeater.DataContext is not BrowserContentViewModelBase vm)
                return;
            
            UpdateSidesheetProperties(vm.Parent, e.AddedItems);
        }

        private void SelectingItemRepeater_OnAttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            if (sender is not CustomItemRepeater itemRepeater)
                return;
            
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
        
        private Dictionary<Scroller, IDisposable>? _toolbarScrollDisposables;

        // Binding observable to opacity mask and update visual after scrolling
        // ReSharper disable once UnusedMember.Local
        private void OnScrollerAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is not Scroller scroller)
                return;
            _toolbarScrollDisposables ??= new Dictionary<Scroller, IDisposable>();

            _toolbarScrollDisposables.Add(scroller, scroller.GetObservable(Visual.OpacityMaskProperty).Subscribe(delegate
            {
                scroller.InvalidateVisual();
            }));
        }

        // Unbind observable when scroller is detached from visual tree
        // ReSharper disable once UnusedMember.Local
        private void OnScrollerDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is not Scroller scroller)
                return;
            
            if(_toolbarScrollDisposables is null)
                return;

            if (_toolbarScrollDisposables.TryGetValue(scroller, out var disposable))
            {
                disposable.Dispose();
                _toolbarScrollDisposables.Remove(scroller);
            }

            if (_toolbarScrollDisposables.Count != 0)
                return;
            
            _toolbarScrollDisposables.Clear();
            _toolbarScrollDisposables = null;
        }

        
    }
}