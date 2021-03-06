using System.Threading;
using Files.ViewModels.Browser.Files.Interfaces;

namespace Files.ViewModels.Browser.Files
{
    public abstract class FileSystemItemViewModel : ItemViewModelBase, IFileSystemViewModel
    {
        private string _fullPath = string.Empty;
        public string FullPath
        {
            get => _fullPath;
            protected set
            {
                _fullPath = value;
                OnPropertyChanged();
            }
        }
        
        private long? _size;

        public long? Size
        {
            get => _size;
            protected set
            {
                _size = value;
                OnPropertyChanged();
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