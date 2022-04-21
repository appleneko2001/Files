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

        private void BrowserViewContextMenu_OnDataContextChanged(object sender, EventArgs e)
        {
            if (sender is not ContextMenu menu)
                return;

            if (menu.DataContext is not ItemViewModelBase vm)
                return;
            
            SetContextMenuOnSelectedItems(menu, vm);
        }

        private void SetContextMenuOnUnselectedState(ContextMenu menu,
            BrowserContentViewModelBase content)
        {
            menu.Items = ContextMenuBackend.Instance.GetContextMenu(content);
        }

        private void SetContextMenuOnSelectedItems(ContextMenu menu, ItemViewModelBase vm)
        {
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
            if (sender is not CustomItemRepeater itemRepeater)
                return;
            _prevItems = _currentItems;
            
            _currentItems = itemRepeater;

            if (itemRepeater.DataContext is not BrowserContentViewModelBase vm)
                return;
            
            UpdateSidesheetProperties(vm.Parent, e.AddedItems);
        }

        private void SelectingItemRepeater_OnAttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            if (sender is not CustomItemRepeater itemRepeater)
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