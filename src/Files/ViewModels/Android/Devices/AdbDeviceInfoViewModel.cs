using Files.Adb.Models.Connections;
using MinimalMvvm.ViewModels;
using AdbDeviceModel = Files.Adb.Models.AdbDeviceModel;

namespace Files.ViewModels.Android.Devices
{
    public class AdbDeviceInfoViewModel : ViewModelBase
    {
        private AdbDeviceModel _model;

        public AdbDeviceInfoViewModel(AdbDeviceModel model)
        {
            _model = model;
        }

        public string Name => _model.Model;
        public IAdbConnection Connection => _model.Connection;
        
        public string ConnectionString => _model.Connection.GetAdbConnectionString();
        
        public bool IsReady => _model.IsReady;
        public bool IsUnauthorized => _model.IsUnauthorized;
    }
}