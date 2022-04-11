using System;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Files.Commands;

namespace Files.ViewModels.Browser.Files.Local
{
    public class FolderItemViewModel : LocalFileSystemItemViewModel
    {
        private static readonly RelayCommand _clickCommand = new(ExecuteOpenFolderCommand);
        public static RelayCommand OpenFolderCommand => _clickCommand;

        public override bool IsFolder => true;

        private static void ExecuteOpenFolderCommand(object obj)
        {
            if (obj is FolderItemViewModel vm)
            {
                vm.Parent.Parent.Open(new Uri(vm.FullPath));
            }
        }

        public FolderItemViewModel(LocalFilesBrowserContentViewModel parent, DirectoryInfo di) : base(parent, di)
        {
            Name = di.Name;
            DisplayName = di.Name;

            FullPath = di.FullName;
        }
        
        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            // TODO: folder previews
        }

        public override ICommand OnClickCommand => _clickCommand;
    }
}