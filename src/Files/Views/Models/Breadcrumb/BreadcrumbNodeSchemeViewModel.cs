using System;
using Material.Icons;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbNodeSchemeViewModel : BreadcrumbNodeWithIconViewModel
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
                    IconKind = MaterialIconKind.Folder;
                } break;
                
                case FtpScheme:
                {
                    Header = "FTP";
                    IconKind = MaterialIconKind.FolderNetworkOutline;
                } break;
                
                case SftpScheme:
                {
                    Header = "Secure FTP";
                    IconKind = MaterialIconKind.FolderKeyNetwork;
                } break;
                
                default:
                {
                    Header = "Unknown";
                    IconKind = MaterialIconKind.HelpNetworkOutline;
                } break;
            }
        }
    }
}