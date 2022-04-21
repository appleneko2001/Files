using System;
using Material.Icons;

namespace Files.ViewModels.Breadcrumb
{
    public class BreadcrumbNodeSchemeViewModel : BreadcrumbNodeWithIconViewModel
    {
        private const string FileScheme = "file";
        private const string FtpScheme = "ftp";
        private const string SftpScheme = "sftp";
        private const string AdbFileScheme = "adbfile";
        
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

                case AdbFileScheme:
                {
                    Header = "Android (Via ADB)";
                    IconKind = MaterialIconKind.Adb;
                } break;
                
                default:
                {
                    Header = "Unknown";
                    IconKind = MaterialIconKind.HelpNetworkOutline;
                } break;
            }
        }
        
        public override void Click()
        {
            // ignored
        }
    }
}