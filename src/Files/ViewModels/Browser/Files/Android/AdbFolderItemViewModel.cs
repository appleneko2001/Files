using System.Windows.Input;
using Files.Models.Android.Storages;
using Files.ViewModels.Browser.Files.Interfaces;
using Files.ViewModels.Context.Menus.Presets;

namespace Files.ViewModels.Browser.Files.Android
{
    public class AdbFolderItemViewModel : AdbFileSystemItemViewModel, IFolderViewModel
    {
        public AdbFolderItemViewModel(BrowserContentViewModelBase parent,
            AdbListFilesItemModel model) : base(parent, model)
        {
        }
        
        public override ICommand OnClickCommand => OpenFolderContextMenuAction.Instance.Command;

        public override bool IsFolder => true;
    }
}