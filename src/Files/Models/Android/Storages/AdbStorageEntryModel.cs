using Files.Adb.Models.Connections;
using Files.Models.Devices.Enums;

namespace Files.Models.Android.Storages
{
    public class AdbStorageEntryModel : StorageEntryModel
    {
        private IAdbConnection _connection;
        private string _entry;
        
        public AdbStorageEntryModel(IAdbConnection connection, string label = "Internal Storage", string path = "/sdcard/")
        {
            _connection = connection;
            
            Label = label;
            EntryKind = DeviceKind.StaticStorage;
            
            UpdateEntry(path);
        }
        
        public override DeviceKind? EntryKind { get; }
        public override string ProcPath { get; }
        public override string Entry => _entry;
        public override string Label { get; }
        public override bool CanUnmount => false;
        public override bool IsReady => true;
        
        protected override void UpdateProcPath(string p)
        {
            throw new System.NotImplementedException();
        }

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