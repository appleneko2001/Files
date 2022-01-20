﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;
using Avalonia.Threading;
using Files.Views.Models.Browser.Files.Local;

namespace Files.Views.Models.Browser
{
    public class LocalFilesBrowserContentViewModel : BrowserContentViewModelBase
    {
        public LocalFilesBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {

        }

        public override void LoadContent(Uri uri, CancellationToken _cancellationToken = default)
        {
            var di = new DirectoryInfo(HttpUtility.UrlDecode(uri.AbsolutePath));
            if (!di.Exists)
                throw new DirectoryNotFoundException($"Directory \"{di.FullName}\" is invalid or not exists.");

            EnumerateDirectoryAndFillCollection(di, _cancellationToken);
        }

        public override void RequestPreviews(CancellationToken _cancellationToken = default)
        {
            /*
            foreach (var item in Content)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                
                if(item is FileItemViewModel file)
                    file.TryGetPreview(_cancellationToken);
            }
            */
        }

        private void EnumerateDirectoryAndFillCollection(DirectoryInfo di, CancellationToken _cancellationToken = default)
        {
            const int maxBulkCount = 8;
            var bulkPool = new ArrayList(maxBulkCount);

            void Commit()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                
                AddItemsOnUiThread(bulkPool);
                bulkPool.Clear();
            }
            
            foreach (var directory in di.EnumerateDirectories())
            { 
                if (bulkPool.Count >= bulkPool.Capacity)
                {
                    Commit();
                }
                bulkPool.Add(new FolderItemViewModel(this, directory));
            }

            if (bulkPool.Count > 0) Commit();

            foreach (var fileInfo in di.EnumerateFiles())
            {
                var file = new FileItemViewModel(this, fileInfo);

                if (bulkPool.Count >= bulkPool.Capacity)
                {
                    Commit();
                }
                
                bulkPool.Add(file);
            }
            
            if (bulkPool.Count > 0)
            {
                Commit();
            }
        }

        private void AddItem(ItemViewModelBase item) => Content.Add(item);

        private void AddItemOnUiThread(ItemViewModelBase item)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                AddItem(item);
            }, DispatcherPriority.Background).Wait();
            
            // Avalonia Input system will be frozen if not adding Thread.Sleep
            Thread.Sleep(1);
        }
        
        private void AddItemsOnUiThread(IEnumerable<ItemViewModelBase> items)
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
        
        private void AddItemsOnUiThread(IEnumerable items)
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