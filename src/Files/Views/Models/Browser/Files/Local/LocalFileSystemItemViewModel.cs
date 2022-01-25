using System;
using System.IO;
using Material.Icons;

namespace Files.Views.Models.Browser.Files.Local
{
    public abstract class LocalFileSystemItemViewModel : ItemViewModelBase
    {
        private MaterialIconKind? _additionalIconKind;
        public MaterialIconKind? AdditionalIconKind => _additionalIconKind;
        
        private string _fullPath;
        public string FullPath
        {
            get => _fullPath;
            protected set
            {
                _fullPath = value;
                RaiseOnPropertyChanged();
            }
        }
        
        protected LocalFileSystemItemViewModel(LocalFilesBrowserContentViewModel parent) : base(parent)
        {
            
        }

        protected LocalFileSystemItemViewModel(LocalFilesBrowserContentViewModel parent, FileSystemInfo info) : base(parent)
        {
            foreach (var attr in (FileAttributes[]) Enum.GetValues(typeof(FileAttributes)))
            {
                if (!info.Attributes.HasFlag(attr))
                    continue;
                
                switch (attr)
                {
                    case FileAttributes.Hidden:
                        IsVisible = false;
                        break;
                
                    case FileAttributes.Encrypted:
                        _additionalIconKind = MaterialIconKind.Lock;
                        break;
                
                    case FileAttributes.System:
                        _additionalIconKind = MaterialIconKind.Cog;
                        break;
                
                    case FileAttributes.Temporary:
                        _additionalIconKind = MaterialIconKind.ClockOutline;
                        break;
                
                    case FileAttributes.Offline:
                        _additionalIconKind = MaterialIconKind.Close;
                        break;
                }
            }
        }
    }
}