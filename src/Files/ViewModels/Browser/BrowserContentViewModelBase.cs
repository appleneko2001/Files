using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Threading;
using Files.Comparers;
using Files.Extensions;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Browser
{
    public abstract class BrowserContentViewModelBase : ViewModelBase, IDisposable
    {
        #region Properties

        public SelectionModel<object?> Selection => _selection;
        
        public BrowserWindowTabViewModel Parent => _parent;
        
        public ObservableCollection<ItemViewModelBase> Content => _content;

        #endregion

        #region Fields

        private readonly SelectionModel<object?> _selection = new()
        {
            SingleSelect = false
        };
        
        private BrowserContentComparer _comparer = new SortByNameComparer();
        
        private BrowserWindowTabViewModel _parent;

        private ObservableCollection<ItemViewModelBase> _content;

        #endregion

        protected BrowserContentViewModelBase(BrowserWindowTabViewModel parent)
        {
            _parent = parent;
            _content = new ObservableCollection<ItemViewModelBase>();

            BindEvents();
        }

        public abstract void LoadContent(Uri uri, CancellationToken _cancellationToken = default);

        protected void AddItem(ItemViewModelBase item) => Content.SortAdd(_comparer, item);

        protected void AddItemOnUiThread(ItemViewModelBase item)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                AddItem(item);
            }, DispatcherPriority.Background).Wait();
            
            // Avalonia Input system will be frozen if not adding Thread.Sleep
            Thread.Sleep(1);
        }
        
        protected void AddItemsOnUiThread(IEnumerable<ItemViewModelBase> items)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                foreach (var item in items)
                {
                    AddItem(item);
                }
            }, DispatcherPriority.Background).Wait();
            
            // Avalonia Input system will be frozen if not adding Thread.Sleep
            Thread.Sleep(1);
        }
        
        protected void AddItemsOnUiThread(IEnumerable items)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                foreach (var item in items)
                {
                    if(item is ItemViewModelBase i)
                        AddItem(i);
                }
            }, DispatcherPriority.Background).Wait();
            
            // Avalonia Input system will be frozen if not adding Thread.Sleep
            Thread.Sleep(1);
        }

        protected virtual void BindEvents()
        {
            _selection.PropertyChanged += OnSelectionPropertyChangedHandler;
            _selection.SelectionChanged += OnSelectionChangedHandler;
            _selection.LostSelection += OnLostSelectionHandler;
        }

        protected virtual void OnLostSelectionHandler(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnSelectionChangedHandler(object sender, SelectionModelSelectionChangedEventArgs<object?> e)
        {
            foreach (var item in e.DeselectedItems)
            {
                if (item is ISelectable i)
                    i.IsSelected = false;
            }

            foreach (var item in e.SelectedItems)
            {
                if (item is ISelectable i)
                    i.IsSelected = true;
            }
        }

        protected virtual void OnSelectionPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void UnbindEvents()
        {
            _selection.PropertyChanged -= OnSelectionPropertyChangedHandler;
            _selection.SelectionChanged -= OnSelectionChangedHandler;
            _selection.LostSelection -= OnLostSelectionHandler;
        }

        /// <summary>
        /// Invoke this when view model is no longer used
        /// </summary>
        public void Dispose()
        {
            UnbindEvents();
        }
    }
}