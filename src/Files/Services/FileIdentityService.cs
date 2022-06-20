using System.IO;
using Avalonia;
using Files.Services.Platform.Interfaces;
using Files.ViewModels;
using FileSignatures;
using FileSignatures.Formats;
using Material.Icons;

namespace Files.Services
{
    public class FileIdentityService
    {
        private static IPlatformSupportGetIcon? _getIconService;
        private static IPlatformSupportExecuteApplication? _executeApplicationService;
        
        private static MaterialIconViewModel _fileIcon;
        private static MaterialIconViewModel _execIcon;
        private static MaterialIconViewModel _videoIcon;
        private static MaterialIconViewModel _musicIcon;
        private static MaterialIconViewModel _pictureIcon;

        private static FileFormatInspector _inspectorInstance;

        static FileIdentityService()
        {
            _inspectorInstance = new FileFormatInspector();

            _pictureIcon = new MaterialIconViewModel(MaterialIconKind.FileImage);
            _musicIcon = new MaterialIconViewModel(MaterialIconKind.FileMusic);
            _videoIcon = new MaterialIconViewModel(MaterialIconKind.FileVideo);
            _execIcon = new MaterialIconViewModel(MaterialIconKind.FileCog);
            _fileIcon = new MaterialIconViewModel(MaterialIconKind.File);
        }

        public static bool DetermineFilePreviewable(FileInfo fi)
        {
            using var stream = fi.OpenRead();

            return DetermineFilePreviewable(stream);
        }

        public static bool DetermineFilePreviewable(Stream stream)
        {
            var format = _inspectorInstance.DetermineFileFormat(stream);

            switch (format)
            {
                case Png:
                case JpegJfif:
                case JpegExif:
                case Jpeg:
                case Gif:
                case Bmp:
                case Image:
                    return true;
            }

            return false;
        }

        public static IconViewModelBase GetIconByExtension(string path)
        {
            _getIconService ??= AvaloniaLocator.Current.GetService<IPlatformSupportGetIcon>();
            _executeApplicationService ??= AvaloniaLocator.Current.GetService<IPlatformSupportExecuteApplication>();
            
            // ReSharper disable once ConvertTypeCheckPatternToNullCheck
            if (_getIconService is null)
                return GetIconByFormat(Path.GetExtension(path));
                
            // Still not ready for use :(
            //if (getIconApi.CanGetIconForFile(path))
            //    return null;
                    
            if (_executeApplicationService is not null && 
                _executeApplicationService.IsExecutableApplication(path))
                return _execIcon;

            return GetIconByFormat(Path.GetExtension(path));
        }

        public static IconViewModelBase GetIconByFormat(string ext)
        {
            //var format = _inspectorInstance.DetermineFileFormat(extension);

            switch (ext.ToLowerInvariant())
            {
                case ".png":
                case ".jfif":
                case ".jpeg":
                case ".jpg":
                case ".gif":
                case ".bmp":
                    return _pictureIcon;

                case ".mp3":
                case ".aac":
                case ".flac":
                case ".m4a":
                case ".wav":
                case ".midi":
                case ".ogg":
                    return _musicIcon;
                
                case ".mp4" :
                case ".avi":
                case ".flv":
                case ".mkv":
                case ".m3u8":
                    return _videoIcon;
            }

            return _fileIcon;
        }
    }
}