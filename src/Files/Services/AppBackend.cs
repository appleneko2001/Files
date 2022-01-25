using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Files.Models.Devices;
using Files.Services.Platform;
using Files.Services.Watchers;
using Files.Views.Models;

namespace Files.Services
{
    public sealed class AppBackend : ViewModelBase
    {
        private PlatformSpecificBridge? _apiBridge = null;
        private static AppBackend _instance;
        private static readonly object _get_instance_lock = new();
        
        public static AppBackend Instance
        {
            get
            {
                lock (_get_instance_lock)
                {
                    return _instance ??= new AppBackend();
                }
            }
        }

        private AppBackend()
        {
            if (Application.Current is FilesApp app)
            {
                app.ApplicationInitializationCompleted += OnApplicationInitializationCompletedHandler;
            }
            
            //_devices = new List<DeviceModel>();
        }

        private void OnApplicationInitializationCompletedHandler(object sender, EventArgs e)
        {
            if (sender is FilesApp app)
            {
                app.ApplicationInitializationCompleted -= OnApplicationInitializationCompletedHandler;
                app.ApplicationShutdown += OnApplicationShutdown;
                
                _apiBridge = app.PlatformApi;
            }
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            if (_platformService != null)
            {
                if (_platformService.DeviceWatcher is DeviceWatcher watcher)
                {
                    watcher.OnDeviceChanged -= OnDeviceWatcherDetectedChanges;
                
                    watcher.Stop();
                }
            }
        }

        // Events
        public event EventHandler DeviceCollectionChanged;

        // Fields
        private OperationSystemService _platformService;
        
        // Public methods
        public void BindServices(OperationSystemService service)
        {
            if (_platformService != null)
                throw new InvalidOperationException("Platform service are bind already. No more needed to bind service.");
            
            _platformService = service;
            
            if (service.DeviceWatcher is DeviceWatcher watcher)
            {
                watcher.OnDeviceChanged += OnDeviceWatcherDetectedChanges;
                
                watcher.Start();
            }
        }

        private void OnDeviceWatcherDetectedChanges(object sender, DeviceChangedEventArgs e)
        {
            // TODO: More better way to handle changes event
            // For now we could use completely refresh collection to apply changes.
            // This way are more savage and painful for computer and users (maybe)
            // But more simple and less glitches.
            
            DeviceCollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Request all visible storage device collection. This action could take longer time to update collection.
        /// </summary>
        public IReadOnlyCollection<DeviceModel>? GetAllStorageDeviceCollection()
        {
            if (_apiBridge == null)
                return null;

            var result = new Collection<DeviceModel>();
            foreach (var deviceEntry in _apiBridge.GetDeviceEntries())
            {
                result.Add(deviceEntry);
            }

            return result;
        }

        public void ShowNativeDialog(string title, string text)
        {
            _platformService.ApiBridge?.PopupMessageWindow(title, text);
        }
    }
}