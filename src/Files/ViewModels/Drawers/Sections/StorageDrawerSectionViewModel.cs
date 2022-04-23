using Files.Models.Devices;
using Files.ViewModels.Drawers.Entries;

namespace Files.ViewModels.Drawers.Sections
{
    public class StorageDrawerSectionViewModel : DrawerSectionViewModel
    {
        private DeviceModel _model;

        public StorageDrawerSectionViewModel(BrowserWindowViewModel parent, DeviceModel model)
            :base(parent)
        {
            _model = model;

            Header = model.Name;
            Icon = new MaterialIconViewModel(model.Icon);

            RefreshStorageEntryCollection();
        }
        
        public DeviceModel GetDeviceModel() => _model;
        
        public void RefreshStorageEntryCollection()
        {
            Entries.Clear();

            foreach (var entry in _model.GetMountedVolumes())
            {
                var vm = new StorageDrawerEntryViewModel(this, entry);
                
                Entries.Add(vm);
            }
        }
    }
}