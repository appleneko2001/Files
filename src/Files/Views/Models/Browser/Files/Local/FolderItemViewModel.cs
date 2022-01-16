using System;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Files.Commands;

namespace Files.Views.Models.Browser.Files.Local
{
    public class FolderItemViewModel : LocalFileSystemItemViewModel
    {
        private static readonly RelayCommand _clickCommand = new(ExecuteOpenFolderCommand);
        public static RelayCommand OpenFolderCommand => _clickCommand;

        private static void ExecuteOpenFolderCommand(object obj)
        {
            if (obj is FolderItemViewModel vm)
            {
                vm.Parent.Parent.Open(new Uri(vm.FullPath));
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
        
        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            // TODO: folder previews
        }

        public override ICommand OnClickCommand => _clickCommand;
    }
}