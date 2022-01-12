using System.IO;
using System.Threading;
using Files.Services;
using Files.Views.Models.Browser.Preview;

namespace Files.Views.Models.Browser.Files.Local
{
    public class FileItemViewModel : LocalFileSystemItemViewModel
    {
        private long _size;
        public long Size => _size;

        private string _fullPath;
        public string FullPath => _fullPath;
        
        private PreviewableViewModelBase? _previewViewModel;
        public PreviewableViewModelBase? Preview => _previewViewModel;
        public bool IsPreviewReady => Preview != null;

        public FileItemViewModel(LocalFilesBrowserContentViewModel parent, FileInfo fi) : base(parent, fi)
        {
            Name = fi.Name;
            DisplayName = fi.Name;
            
            _size = fi.Length;
            _fullPath = fi.FullName;
        }

        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            PreviewManagerBackend.Instance?.ScheduleGetPreview(new FileInfo(_fullPath), OnCompleteGetPreviewTask, _cancellationToken);
        }

        private void OnCompleteGetPreviewTask(PreviewableViewModelBase model)
        {
            _previewViewModel = model;
            RaiseOnPropertyChanged(nameof(Preview));
            RaiseOnPropertyChanged(nameof(IsPreviewReady));
        }
    }
}