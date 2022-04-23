using Files.Adb.Models;
using Files.Models.Devices.Enums;

namespace Files.Models.Android.Storages
{
    public sealed class AdbStorageEntryModel : StorageEntryModel
    {
        private readonly AdbConnection _connection;
        private string _entry;
        
        public AdbStorageEntryModel(AdbConnection connection, string label = "Internal Storage", string path = "/sdcard/")
        {
            _connection = connection;
            _entry = string.Empty;
            
            Label = label;
            EntryKind = DeviceKind.StaticStorage;
            
            UpdateEntry(path);
        }
        
        public override DeviceKind? EntryKind { get; }
        public override string Entry => _entry;
        public override string Label { get; }
        public override bool CanUnmount => false;
        public override bool IsReady => true;

        protected override void UpdateEntry(string e)
        {
            var basePath = _connection.GetConnectionUri()
                .ToString()
                .TrimEnd('/');

            _entry = $"{basePath}{e}";
        }

        protected override void UpdateLabel(string l)
        {
            throw new System.NotImplementedException();
        }

        public override void Unmount()
        {
            throw new System.NotImplementedException();
        }
    }
}