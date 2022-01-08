using System.Collections.Generic;
using System.Collections.ObjectModel;

using Material.Icons;

namespace Files.Models.Devices
{
    public abstract class DeviceModel
    {
        protected DeviceModel(DeviceInfo di)
        {
            _deviceInfo = di;
        }

        private DeviceInfo _deviceInfo;
        public string Name => _deviceInfo.Name;

        public abstract bool IsRemovable { get; }
        
        public abstract bool IsReadonly { get; }
        
        public abstract MaterialIconKind Icon { get; }

        public abstract IReadOnlyCollection<StorageEntryModel> GetMountedVolumes();
    }
}