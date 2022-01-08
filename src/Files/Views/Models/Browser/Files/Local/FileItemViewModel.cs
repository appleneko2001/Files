using System.IO;

namespace Files.Views.Models.Browser.Files.Local
{
    public class FileItemViewModel : LocalFileSystemItemViewModel
    {
        private long _size;
        public long Size => _size;

        private string _fullPath;
        public string FullPath => _fullPath;
        
        public FileItemViewModel(LocalFilesBrowserContentViewModel parent, FileInfo fi) : base(parent, fi)
        {
            Name = fi.Name;
            DisplayName = fi.Name;
            
            _size = fi.Length;
            _fullPath = fi.FullName;
        }
    }
}