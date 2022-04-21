using System;
using System.Windows.Input;
using Files.Models.Android.Storages;
using Files.Services;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Browser.Files.Android
{
    public class AdbFolderItemViewModel : AdbFileSystemItemViewModel
    {
        public AdbFolderItemViewModel(BrowserContentViewModelBase parent,
            AdbListFilesItemModel model) : base(parent, model)
        {
        }
        
        public override ICommand OnClickCommand => CommandsBackend.Instance.OpenFolderInCurrentViewCommand;

        public override bool IsFolder => true;
    }
}