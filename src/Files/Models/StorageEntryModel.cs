using Files.Models.Devices.Enums;

namespace Files.Models
{
    public abstract class StorageEntryModel
    {
        public abstract DeviceKind? EntryKind { get; }

        public abstract string Entry { get; }
        
        public abstract string Label { get; }
        
        public abstract bool CanUnmount { get; }
        
        public abstract bool IsReady { get; }

        protected abstract void UpdateEntry(string e);

        protected abstract void UpdateLabel(string l);

        public abstract void Unmount();
    }
}