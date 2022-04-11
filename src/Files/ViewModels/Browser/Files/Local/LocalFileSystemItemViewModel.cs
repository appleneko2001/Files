using System;
using System.IO;
using Files.ViewModels.Browser.Files.Android;
using Material.Icons;

namespace Files.ViewModels.Browser.Files.Local
{
    public abstract class LocalFileSystemItemViewModel : FileSystemItemViewModel
    {
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
                        AdditionalIconKind = MaterialIconKind.Lock;
                        break;
                
                    case FileAttributes.System:
                        AdditionalIconKind = MaterialIconKind.Cog;
                        break;
                
                    case FileAttributes.Temporary:
                        AdditionalIconKind = MaterialIconKind.ClockOutline;
                        break;
                
                    case FileAttributes.Offline:
                        AdditionalIconKind = MaterialIconKind.Close;
                        break;
                }
            }
        }
    }
}