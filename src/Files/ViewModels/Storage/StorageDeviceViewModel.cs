using System.Collections.ObjectModel;
using Files.Models.Devices;
using Material.Icons;

namespace Files.ViewModels.Storage
{
    public class StorageDeviceViewModel : ViewModelBase
    {
        private BrowserWindowViewModel _parent;
        private DeviceModel _model;
        private MaterialIconKind _iconKind;
        private ObservableCollection<StorageEntryViewModel> _storageEntries;

        public BrowserWindowViewModel Parent => _parent;
        public string Name => _model.Name;
        public MaterialIconKind IconKind => _iconKind;
        public ObservableCollection<StorageEntryViewModel> StorageEntries => _storageEntries;
        public DeviceModel GetDeviceModel() => _model;        
        public StorageDeviceViewModel(BrowserWindowViewModel parent, DeviceModel model)
        {
            _parent = parent;
            _model = model;
            _iconKind = model.Icon;

            _storageEntries = new ObservableCollection<StorageEntryViewModel>();
            RefreshStorageEntryCollection();
        }

        public void RefreshStorageEntryCollection()
        {
            StorageEntries.Clear();

            foreach (var entry in _model.GetMountedVolumes())
            {
                StorageEntries.Add(new StorageEntryViewModel(this, entry));
            }
        }
    }
}