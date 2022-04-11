using System.Threading.Tasks;
using Files.Adb.Models;
using Files.Models.Android;
using Files.Models.Android.Props;
using Files.Models.Devices;
using Files.Models.Devices.Enums;
using Files.Services.Android;

namespace Files.ViewModels.Android.Devices
{
    public class AdbDeviceViewModel : ViewModelBase
    {
        private AdbDeviceInfoViewModel _adbDeviceInfo;
        private AdbDeviceProps _adbDeviceProps;
        
        private string? _deviceName;

        public AdbDeviceViewModel(AdbDeviceModel model)
        {
            _adbDeviceInfo = new AdbDeviceInfoViewModel(model);
            _adbDeviceProps = new AdbDeviceProps();

            GetDeviceProps().Wait(50);
        }

        public string? DeviceName
        {
            get => _deviceName;
            private set
            {
                _deviceName = value;
                RaiseOnPropertyChangedThroughUiThread();
            }
        }

        public AdbDeviceInfoViewModel AdbDeviceInfo => _adbDeviceInfo;

        public DeviceModel GetDeviceModel()
        {
            return new AdbStorageDeviceModel(_adbDeviceInfo.Connection ,new DeviceInfo
            {
                Kind = DeviceKind.UsbStorage,
                Name = DeviceName
            });
        }

        public async Task GetDeviceProps()
        {
            var backend = AndroidDebugBackend.Instance;
            await foreach (var pair in backend.GetPropertiesAsync(AdbDeviceInfo.Connection, "ro.product."))
            {
                _adbDeviceProps.AddOrUpdate(pair);
            }

            DeviceName = _adbDeviceProps.GetBrandModel();
        }
    }
}