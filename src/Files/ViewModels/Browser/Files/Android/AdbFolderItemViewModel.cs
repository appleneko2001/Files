using System.Windows.Input;
using Files.Models.Android.Storages;
using Files.Services;
using Files.ViewModels.Browser.Files.Interfaces;

namespace Files.ViewModels.Browser.Files.Android
{
    public class AdbFolderItemViewModel : AdbFileSystemItemViewModel, IFolderViewModel
    {
        public AdbFolderItemViewModel(BrowserContentViewModelBase parent,
            AdbListFilesItemModel model) : base(parent, model)
        {
        }
        
        public override ICommand OnClickCommand => CommandsBackend.Instance.OpenFolderInCurrentViewCommand;

        public override bool IsFolder => true;
    }
}