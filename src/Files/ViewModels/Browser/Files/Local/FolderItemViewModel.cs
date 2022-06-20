using System.IO;
using System.Threading;
using System.Windows.Input;
using Files.ViewModels.Browser.Files.Interfaces;
using Files.ViewModels.Context.Menus.Presets;

namespace Files.ViewModels.Browser.Files.Local
{
    public class FolderItemViewModel : LocalFileSystemItemViewModel, IFolderViewModel
    {
        public override bool IsFolder => true;

        public FolderItemViewModel(LocalFilesBrowserContentViewModel parent, DirectoryInfo di) : base(parent, di)
        {
            Name = di.Name;
            DisplayName = di.Name;

            FullPath = di.FullName;
        }
        
        public override void TryGetPreview(CancellationToken cancellationToken = default)
        {
            // TODO: folder previews
        }

        public override ICommand OnClickCommand => OpenFolderContextMenuAction.Instance.Command;
    }
}