using System.IO;
using System.Text;
using Files.Extensions;
using Files.ViewModels.Browser.Files.Local;

namespace Files.ViewModels.Browser.Properties
{
    public class CommonFileSystemProperties : FileSystemPropertiesViewModelBase
    {
        private const string NotAvailableString = "N/A";
        
        public CommonFileSystemProperties(LocalFileSystemItemViewModel entry)
        {
            GetFileSystemEntryInformation(entry);
        }

        private void GetFileSystemEntryInformation(LocalFileSystemItemViewModel entry)
        {
            var path = entry.FullPath;
            if (File.Exists(path))
            {
                var fi = new FileInfo(path);
                Name = fi.Name;
                
                // TODO: improve get size on disk / get file size
                Size = $"{fi.Length.HumanizeSizeString()} ({fi.Length.ToString()} bytes)";
                ActualSize = $"{fi.Length.HumanizeSizeString()} ({fi.Length.ToString()} bytes)";
            }
            else if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);
                var builder = new StringBuilder("Folder: ");
                builder.Append(di.Name);

                Name = builder.ToString();
                builder.Clear();

                Size = NotAvailableString;
                ActualSize = NotAvailableString;

                // TODO: Get files and directories count, sizes on disk
                //var walkDirModel = new WalkDirectoryModel();
                /*
                ScheduleTask(delegate
                {  
                });*/
            }
            IsLoading = false;
        }

        private void ScheduleTask()
        {
            
        }
    }
}