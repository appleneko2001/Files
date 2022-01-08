using System;
using System.Collections.ObjectModel;
using Avalonia;
using Files.Services;
using Files.Views.Models.Storage;

namespace Files.Views.Models
{
    public class BrowserWindowViewModel : ViewModelBase
    {
        private BrowserWindow _parentWindow;

        private AppBackend _context;
        private ObservableCollection<StorageDeviceViewModel> _storageDevices;
        private ObservableCollection<BrowserWindowTabViewModel> _tabsViewModel;
        private bool _isNavigationDrawerOpen;
        private BrowserWindowTabViewModel _selectedTab;
        
        public AppBackend Context => _context;
        public BrowserWindow ParentWindow => _parentWindow;
        
        public ObservableCollection<StorageDeviceViewModel> StorageDevices => _storageDevices;
        public ObservableCollection<BrowserWindowTabViewModel> TabViewModel => _tabsViewModel;

        public BrowserWindowTabViewModel SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                RaiseOnPropertyChangedThroughUiThread();
            }
        }

        public bool IsNavigationDrawerOpen
        {
            get => _isNavigationDrawerOpen;
            set
            {
                _isNavigationDrawerOpen = value;
                RaiseOnPropertyChanged();
            }
        }

        public BrowserWindowViewModel(AppBackend context, BrowserWindow parentWindow)
        {
            _context = context;
            _parentWindow = parentWindow;
            _storageDevices = new ObservableCollection<StorageDeviceViewModel>();
            _tabsViewModel = new ObservableCollection<BrowserWindowTabViewModel>();

            SelectedTab = new BrowserWindowTabViewModel(this);

            if (Application.Current is FilesApp app)
            {
                app.ApplicationInitializationCompleted += OnApplicationInitializationCompleted;
            }
        }

        public void RefreshStorageDevicesCollection()
        {
            var result = _context.GetAllStorageDeviceCollection();
            if (result is null)
                return;
            
            StorageDevices.Clear();
            foreach (var device in result)
            {
                StorageDevices.Add(new StorageDeviceViewModel(this, device));
            }
            
            RaiseOnPropertyChangedThroughUiThread(nameof(StorageDevices));
        }

        private void OnApplicationInitializationCompleted(object sender, EventArgs e)
        {
            if (sender is FilesApp app)
            {
                app.ApplicationInitializationCompleted -= OnApplicationInitializationCompleted;
                app.ApplicationShutdown += OnApplicationShutdown;
                
                _context.DeviceCollectionChanged += OnContextDeviceCollectionChanged;

                RefreshStorageDevicesCollection();
            }
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            if (sender is FilesApp app)
            {
                app.ApplicationShutdown -= OnApplicationShutdown;
                
                _context.DeviceCollectionChanged -= OnContextDeviceCollectionChanged;
            }
        }

        private void OnContextDeviceCollectionChanged(object sender, EventArgs e)
        {
            RefreshStorageDevicesCollection();
        }
    }
}