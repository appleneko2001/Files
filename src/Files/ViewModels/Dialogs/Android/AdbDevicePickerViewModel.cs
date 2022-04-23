using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Files.Services.Android;
using Files.ViewModels.Android.Devices;
using MinimalMvvm.ViewModels;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Dialogs.Android
{
    public class AdbDevicePickerViewModel : ViewModelBase
    {
        public ObservableCollection<AdbDeviceViewModel> Devices { get; } = new();
        
        public AdbDeviceViewModel? SelectedDevice { get; set; }
        
        public RelayCommand GetDevicesCommand { get; }

        private readonly AndroidDebugBackend _inst = AndroidDebugBackend.Instance;
        
        public AdbDevicePickerViewModel()
        {
            _inst.UpdateDevicesEvent += OnUpdateDevicesEvent;
            
            GetDevicesCommand = new RelayCommand(ExecuteGetDevicesCommand);

            foreach (var device in _inst.Devices)
            {
                AddDevice(device);
            }
        }

        private void OnUpdateDevicesEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (string item in e.NewItems)
                        AddDevice(item);
                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in Devices.Where(
                                 model => e.OldItems.Cast<string>().Contains(model.Connection.ToString())))
                        Devices.Remove(item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void ExecuteGetDevicesCommand(object? obj)
        {
            Devices.Clear();

            await foreach (var device in _inst.GetDevicesAsync())
            {
                AddDevice(device);
            }
        }

        private void AddDevice(string device)
        {
            if(Devices.Any(model => device ==
                model.Connection.GetAdbConnectionString()))
                return;

            var vm = new AdbDeviceViewModel(device);
            Devices.Add(vm);

            Task.Factory.StartNew(vm.GetDeviceProps);
        }

        public void UnloadEvents()
        {
            _inst.UpdateDevicesEvent -= OnUpdateDevicesEvent;
        }
    }
}