using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Files.Models;
using Files.Models.Devices;
using Files.Models.Devices.Enums;
using Material.Icons;

namespace Files.Windows.Models
{
    // TODO: Suitable device model for Windows OS
    public class MyComputerDeviceModel : DeviceModel
    {
        public MyComputerDeviceModel() : base(new DeviceInfo
        {
            Name = "My computer",
            Kind = DeviceKind.Unknown
        })
        {
            _devices = new ObservableCollection<StorageEntryModel>();
            foreach (var entry in GetMountedVolumes())
            {
                _devices.Add(entry);
            }
        }
        
        public override bool IsRemovable => false;
        public override bool IsReadonly => false;
        public override MaterialIconKind Icon => MaterialIconKind.Computer;

        public override IReadOnlyCollection<StorageEntryModel> GetMountedVolumes()
        {
            var result = new Collection<StorageEntryModel>();
            
            var drives = DriveInfo.GetDrives();
            foreach (var driveInfo in drives)
            {
                result.Add(new WindowsMediaEntryModel(driveInfo));
            }

            return result;
        }

        private ObservableCollection<StorageEntryModel> _devices;
    }
}