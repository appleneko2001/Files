using System.Threading;

namespace Files.ViewModels.Browser.Files
{
    public abstract class FileSystemItemViewModel : ItemViewModelBase
    {
        private string _fullPath = string.Empty;
        public string FullPath
        {
            get => _fullPath;
            protected set
            {
                _fullPath = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private long? _size;

        public long? Size
        {
            get => _size;
            protected set
            {
                _size = value;
                RaiseOnPropertyChanged();
            }
        }
        
        public virtual void PreAction(CancellationToken token)
        {
            // Do nothing
        }

        public FileSystemItemViewModel(BrowserContentViewModelBase parent) : base(parent)
        {
        }

        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            // Ignore
        }
    }
}