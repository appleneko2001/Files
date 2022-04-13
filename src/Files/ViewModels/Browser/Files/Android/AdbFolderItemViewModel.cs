using System;
using System.Windows.Input;
using Files.Models.Android.Storages;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Browser.Files.Android
{
    public class AdbFolderItemViewModel : AdbFileSystemItemViewModel
    {
        private static readonly RelayCommand _clickCommand = new(ExecuteOpenFolderCommand);
        public static RelayCommand OpenFolderCommand => _clickCommand;
        
        private static void ExecuteOpenFolderCommand(object? obj)
        {
            if (obj is AdbFolderItemViewModel vm)
            {
                if (vm.Parent is AdbBrowserContentViewModel parent)
                {
                    vm.Parent.Parent.Open(new Uri(parent.Connection.GetConnectionUri(), "."+vm.FullPath));
                }
            }
        }
        
        public AdbFolderItemViewModel(BrowserContentViewModelBase parent,
            AdbListFilesItemModel model) : base(parent, model)
        {
        }
        
        public override ICommand OnClickCommand => _clickCommand;

        public override bool IsFolder => true;
    }
}