using System.Threading;
using Files.Adb.Models;
using Files.Models.Android;
using Files.Models.Android.Props;
using Files.Models.Devices;
using Files.Models.Devices.Enums;
using Files.Services.Android;
using MinimalMvvm.Extensions;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Android.Devices
{
    public class AdbDeviceViewModel : ViewModelBase
    {
        private readonly AdbDeviceProps _adbDeviceProps;
        private readonly AdbConnection _connection;
        private string? _deviceName = "RETRIEVING";

        public AdbDeviceViewModel(AdbDeviceModel model)
        {
            _adbDeviceProps = new AdbDeviceProps();
            _connection = model.Connection;
        }

        public AdbDeviceViewModel(string device)
        {
            if (device.Contains('\t'))
                device = device.Split('\t')[0];
            
            _adbDeviceProps = new AdbDeviceProps();
            _connection = new AdbConnection(device);
        }

        public string? DeviceName
        {
            get => _deviceName;
            private set => this.SetAndUpdateIfChanged(ref _deviceName, value);
        }

        public DeviceModel GetDeviceModel()
        {
            return new AdbStorageDeviceModel(Connection, new DeviceInfo
            {
                Kind = DeviceKind.UsbStorage,
                Name = DeviceName!
            });
        }

        public void GetDeviceProps()
        {
            var backend = AndroidDebugBackend.Instance;
            
            for (var retry = 0; retry < 3; retry++)
            {
                try
                {
                    foreach (var pair in backend
                                 .GetProperties(Connection, "ro.product."))
                    {
                        _adbDeviceProps.AddOrUpdate(pair);
                    }

                    DeviceName = _adbDeviceProps.GetBrandModel();
                    
                    break;
                }
                catch
                {
                    DeviceName = "Unable to retrieve info.";
                    
                    Thread.Sleep(100);
                }
            }
        }

        public AdbConnection Connection => _connection;
    }
}