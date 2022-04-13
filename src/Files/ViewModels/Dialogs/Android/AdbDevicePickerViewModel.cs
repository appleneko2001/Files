using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
        
        public AdbDevicePickerViewModel()
        {
            GetDevicesCommand = new RelayCommand(ExecuteGetDevicesCommand);
        }

        private async void ExecuteGetDevicesCommand(object? obj)
        {
            Devices.Clear();
            
            Thread.Sleep(1000);
            
            await foreach (var device in AndroidDebugBackend.Instance.GetDevicesAsync())
            {
                if(Devices.All(model => device.Connection.GetAdbConnectionString() 
                                        != model.AdbDeviceInfo.Connection.GetAdbConnectionString()))
                    Devices.Add(new AdbDeviceViewModel(device));
            }
        }
    }
}