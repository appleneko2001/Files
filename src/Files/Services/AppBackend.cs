using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Files.Models.Devices;
using Files.Models.Messenger;
using Files.Services.Platform.Interfaces;
using Files.Services.Watchers;
using MinimalMvvm.ViewModels;

namespace Files.Services
{
    public sealed class AppBackend : ViewModelBase
    {
        private FilesApp _appInstance;
        private static AppBackend? _instance;
        private static readonly object GetInstanceLock = new();
        
        public static AppBackend Instance
        {
            get
            {
                lock (GetInstanceLock)
                {
                    return _instance ??= new AppBackend();
                }
            }
        }

        private AppBackend()
        {
            if (Application.Current is FilesApp app)
            {
                _appInstance = app;
                app.ApplicationInitializationCompleted += OnApplicationInitializationCompletedHandler;
            }
        }

        private void OnApplicationInitializationCompletedHandler(object sender, EventArgs e)
        {
            if (sender is not FilesApp app)
                return;
            
            app.ApplicationInitializationCompleted -= OnApplicationInitializationCompletedHandler;
            app.ApplicationShutdown += OnApplicationShutdown;
            
            Messenger.Subscribe<OpenFolderInNewWindowRequestModel>(OnReceiveOpenFolderInNewWindowRequest);

            var deviceWatcher = AvaloniaLocator.Current.GetService<IPlatformSupportDeviceWatcherService>();

            if (deviceWatcher == null)
                return;

            deviceWatcher.OnDeviceChanged += OnDeviceWatcherDetectedChanges;
            deviceWatcher.Start();
        }

        private void OnReceiveOpenFolderInNewWindowRequest(OpenFolderInNewWindowRequestModel param)
        {
            _appInstance.CreateBrowserWindow(param.FolderUri).Show();
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            var service = AvaloniaLocator
                .Current
                .GetService<IPlatformSupportDeviceWatcherService>();

            if (service == null)
                return;
            
            service.OnDeviceChanged -= OnDeviceWatcherDetectedChanges;
            service.Stop();
        }

        // Events
        public event EventHandler? DeviceCollectionChanged;

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
            var service = AvaloniaLocator.Current.GetService<IPlatformSupportDeviceEntries>();
            
            if (service == null)
                return null;

            var result = new Collection<DeviceModel>();
            foreach (var deviceEntry in service.GetDeviceEntries())
            {
                result.Add(deviceEntry);
            }

            return result;
        }
    }
}