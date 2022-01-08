namespace Files.Models
{
    public abstract class StorageEntryModel
    {
        public abstract string ProcPath { get; }
        
        public abstract string Entry { get; }
        
        public abstract string Label { get; }
        
        public abstract bool CanUnmount { get; }
        
        public abstract bool IsReady { get; }

        protected abstract void UpdateProcPath(string p);

        protected abstract void UpdateEntry(string e);

        protected abstract void UpdateLabel(string l);

        public abstract void Unmount();
    }
}