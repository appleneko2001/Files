using System;
using Material.Icons;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbNodeSchemeViewModel : BreadcrumbNodeViewModel
    {
        private const string FileScheme = "file";
        private const string FtpScheme = "ftp";
        private const string SftpScheme = "sftp";
        
        
        public BreadcrumbNodeSchemeViewModel(BreadcrumbPathViewModel parent, int index, string path) : base(parent, index)
        {
            Path = path;
            var scheme = path.ToLowerInvariant().Replace(Uri.SchemeDelimiter, "");

            switch (scheme)
            {
                case FileScheme:
                {
                    Header = "File system";
                    _iconKind = MaterialIconKind.Folder;
                } break;
                
                case FtpScheme:
                {
                    Header = "FTP";
                    _iconKind = MaterialIconKind.FolderNetworkOutline;
                } break;
                
                case SftpScheme:
                {
                    Header = "Secure FTP";
                    _iconKind = MaterialIconKind.FolderKeyNetwork;
                } break;
                
                default:
                {
                    Header = "Unknown";
                    _iconKind = MaterialIconKind.HelpNetworkOutline;
                } break;
            }
        }

        private readonly MaterialIconKind? _iconKind;
        public MaterialIconKind? IconKind => _iconKind;
    }
}