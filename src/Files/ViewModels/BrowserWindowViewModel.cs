using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Files.Models.Android;
using Files.Services;
using Files.Services.Android;
using Files.ViewModels.Android.Devices;
using Files.ViewModels.Dialogs.Android;
using Files.ViewModels.Storage;
using Files.Views;
using Files.Views.Dialogs;
using Material.Dialog;
using Material.Dialog.Enums;
using MinimalMvvm.ViewModels;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels
{
    public class BrowserWindowViewModel : ViewModelBase
    {
        private static RelayCommand _newTabCommand = new(delegate(object o)
        {
            if (o is BrowserWindowViewModel vm)
            {
                OnExecuteNewTabAndSelectCommand(vm);
            }
        });

        private static RelayCommand _fullscreenCommand = new(delegate(object o)
        {
            if (o is Window w)
            {
                w.WindowState = w.WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
            }
        });

        private static RelayCommand _connectPhoneViaAdbCommand = new(delegate(object o)
        {
            if (o is BrowserWindowViewModel vm)
            {
                OnExecuteConnectPhoneViaAdbCommand(vm);
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

        public RelayCommand ConnectPhoneViaAdbCommand => _connectPhoneViaAdbCommand;

        public RelayCommand FullscreenCommand => _fullscreenCommand;

        public RelayCommand NewTabCommand => _newTabCommand;

        public BrowserWindowTabViewModel? SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != null)
                    _selectedTab.IsSelected = false;

                value ??= TabsViewModel.Last();

                _selectedTab = value;
                OnPropertyChanged();

                if (value != null)
                    value.IsSelected = true;
            }
        }

        public bool IsNavigationDrawerOpen
        {
            get => _isNavigationDrawerOpen;
            set
            {
                _isNavigationDrawerOpen = value;
                OnPropertyChanged();
            }
        }

        public BrowserWindowViewModel(AppBackend context, BrowserWindow parentWindow)
        {
            _context = context;
            _parentWindow = parentWindow;
            _storageDevices = new ObservableCollection<StorageDeviceViewModel>();
            _tabsViewModel = new ObservableCollection<BrowserWindowTabViewModel>();

            OnExecuteNewTabAndSelectCommand(this);
            
            ParentWindow.Closed += OnWindowClosed;

            if (Application.Current is FilesApp app)
            {
                if (app.Initialized)
                {
                    OnApplicationInitializationCompleted(app, EventArgs.Empty);
                    return;
                }

                app.ApplicationInitializationCompleted += OnApplicationInitializationCompleted;
            }
        }

        private static void OnExecuteNewTabAndSelectCommand(BrowserWindowViewModel vm)
        {
            var newTab = new BrowserWindowTabViewModel(vm);
            vm.TabsViewModel.Add(newTab);
            vm.SelectedTab = newTab;
        }

        private static async void OnExecuteConnectPhoneViaAdbCommand(BrowserWindowViewModel vm)
        {
            var context = new AdbDevicePickerViewModel();
            var content = new AdbDevicePickerDialog
            {
                DataContext = context
            };
            var dialog = DialogHelper.CreateCustomDialog(new CustomDialogBuilderParams
            {
                ContentHeader = "Connect to Android device",
                DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.OkCancel),
                Content = content
            });

            void UpdateDevicesListHandler(object? sender, EventArgs e)
            {
                context.GetDevicesCommand.Execute(null);
            }

            AndroidDebugBackend.Instance.UpdateDevicesEvent += UpdateDevicesListHandler;
            
            UpdateDevicesListHandler(null, EventArgs.Empty);
            
            var result = await dialog.ShowDialog(vm.ParentWindow);

            AndroidDebugBackend.Instance.UpdateDevicesEvent -= UpdateDevicesListHandler;
            
            if (result.GetResult != "ok")
                return;

            var device = context.SelectedDevice;

            if (device == null)
                return;

            // Do not connect to the same device twice
            if (vm.StorageDevices.Any(
                    delegate(StorageDeviceViewModel model)
                    {
                        if (model.GetDeviceModel() is not AdbStorageDeviceModel d)
                            return false;

                        return device.AdbDeviceInfo.Connection == d.Connection;
                    }))
                return;

            device.GetDeviceProps();
            vm.StorageDevices.Add(new StorageDeviceViewModel(vm, device.GetDeviceModel()));
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
            if (sender is not FilesApp app)
                return;
            
            app.ApplicationInitializationCompleted -= OnApplicationInitializationCompleted;
            app.ApplicationShutdown += OnApplicationShutdown;

            UseHandlerWhileWindowAlive();

            RefreshStorageDevicesCollection();
        }

        private void UseHandlerWhileWindowAlive()
        {
            TabsViewModel.CollectionChanged += OnTabsViewModelCollectionChanged;
            
            _context.DeviceCollectionChanged += OnContextDeviceCollectionChanged;
        }

        private void OnTabsViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var tab in TabsViewModel)
            {
                tab.CloseTabCommand.RaiseCanExecuteChanged();
                break;
            }
        }

        private void RemoveHandlerAfterWindowClosed()
        {
            TabsViewModel.CollectionChanged -= OnTabsViewModelCollectionChanged;
            
            _context.DeviceCollectionChanged -= OnContextDeviceCollectionChanged;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            RemoveHandlerAfterWindowClosed();
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            if (sender is not FilesApp app)
                return;
            
            app.ApplicationShutdown -= OnApplicationShutdown;
        }

        private void OnContextDeviceCollectionChanged(object sender, EventArgs e)
        {
            RefreshStorageDevicesCollection();
        }
    }
}