using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Avalonia.Threading;
using Files.Comparers;
using Files.Extensions;

namespace Files.ViewModels.Browser
{
    public abstract class BrowserContentViewModelBase : ViewModelBase
    {
        private BrowserContentComparer _comparer = new SortByNameComparer();
        
        private BrowserWindowTabViewModel _parent;
        public BrowserWindowTabViewModel Parent => _parent;
        
        private ObservableCollection<ItemViewModelBase> _content;
        public ObservableCollection<ItemViewModelBase> Content => _content;

        protected BrowserContentViewModelBase(BrowserWindowTabViewModel parent)
        {
            _parent = parent;
            _content = new ObservableCollection<ItemViewModelBase>();
        }

        public abstract void LoadContent(Uri uri, CancellationToken _cancellationToken = default);

        public abstract void RequestPreviews(CancellationToken _cancellationToken = default);
        
        
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
    }
}