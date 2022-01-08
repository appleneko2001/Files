using System;
using System.IO;
using Files.Commands;

namespace Files.Views.Models.Browser.Files.Local
{
    public class FolderItemViewModel : LocalFileSystemItemViewModel
    {
        private static RelayCommand _clickCommand = new(ExecuteOpenFolderCommand);

        private static void ExecuteOpenFolderCommand(object obj)
        {
            if (obj is FolderItemViewModel vm)
            {
                vm.Parent.Parent.OpenAsync(new Uri(vm.FullPath));
            }
        }

        private string _fullPath;
        public string FullPath => _fullPath;

        public FolderItemViewModel(LocalFilesBrowserContentViewModel parent, DirectoryInfo di) : base(parent, di)
        {
            Name = di.Name;
            DisplayName = di.Name;

            _fullPath = di.FullName;
        }

        public override RelayCommand OnClickCommand => _clickCommand;
    }
}