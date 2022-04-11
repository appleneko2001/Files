using System.Collections.Generic;
using Files.Adb.Models.Connections;
using Files.Models.Android.Storages;
using Files.Models.Devices;
using Material.Icons;

namespace Files.Models.Android
{
    public class AdbStorageDeviceModel : DeviceModel
    {
        private readonly IAdbConnection _connection;
        
        public AdbStorageDeviceModel(IAdbConnection conn, DeviceInfo di) : base(di)
        {
            _connection = conn;
        }

        public override bool IsRemovable => true;
        public override bool IsReadonly => false;
        public override MaterialIconKind Icon => MaterialIconKind.Android;
        
        public IAdbConnection Connection => _connection;
        
        // TODO: Add sdcard storage if exists
        public override IReadOnlyCollection<StorageEntryModel> GetMountedVolumes()
        {
            return new List<StorageEntryModel>
            {
                new AdbStorageEntryModel(_connection)
            };
        }
    }
}