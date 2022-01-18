using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Files.Commands;
using Files.Services;
using Files.Views.Models.Storage;

namespace Files.Views.Models
{
    public class BrowserWindowViewModel : ViewModelBase
    {
        private static RelayCommand _newTabCommand = new RelayCommand(delegate(object o)
        {
            if (o is BrowserWindowViewModel vm)
            {
                OnExecuteNewTabAndSelectCommand(vm);
            }
        });
        
        private static RelayCommand _fullscreenCommand = new RelayCommand(delegate(object o)
        {
            if (o is Window w)
            {
                w.WindowState = w.WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
            }
        });
        
        private BrowserWindow _parentWindow;

        private AppBackend _context;
        private ObservableCollection<StorageDeviceViewModel> _storageDevices;
        private ObservableCollection<BrowserWindowTabViewModel> _tabsViewModel;
        private bool _isNavigationDrawerOpen;
        private BrowserWindowTabViewModel? _selectedTab;
        
        private BrowserWindowTabViewModel _previousSelectedTab;
        //private SelectionModel<BrowserWindowTabViewModel> _tabSelection;

        public AppBackend Context => _context;
        public BrowserWindow ParentWindow => _parentWindow;
        
        public ObservableCollection<StorageDeviceViewModel> StorageDevices => _storageDevices;
        public ObservableCollection<BrowserWindowTabViewModel> TabsViewModel => _tabsViewModel;

        public RelayCommand FullscreenCommand => _fullscreenCommand;
                
        public RelayCommand NewTabCommand => _newTabCommand;

        public BrowserWindowTabViewModel? SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != null)
                    _selectedTab.IsSelected = false;
                
                _selectedTab = value;
                RaiseOnPropertyChangedThroughUiThread();
                
                if(value != null)
                    value.IsSelected = true;
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

            OnExecuteNewTabAndSelectCommand(this);

            if (Application.Current is FilesApp app)
            {
                app.ApplicationInitializationCompleted += OnApplicationInitializationCompleted;
            }
        }

        private static void OnExecuteNewTabAndSelectCommand(BrowserWindowViewModel vm)
        {
            var newTab = new BrowserWindowTabViewModel(vm);
            vm.TabsViewModel.Add(newTab);
            vm.SelectedTab = newTab;
        }

        internal void CloseTab(BrowserWindowTabViewModel tab)
        {
            TabsViewModel.Remove(tab);
            
            var lastTab = TabsViewModel.LastOrDefault();
            if (lastTab == tab)
                lastTab = null;
            SelectedTab = lastTab;
            
            tab.AfterClose();
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