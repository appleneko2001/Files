using System.IO;
using System.Threading;
using System.Windows.Input;
using Files.Services;

namespace Files.ViewModels.Browser.Files.Local
{
    public class FolderItemViewModel : LocalFileSystemItemViewModel
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

        public override ICommand OnClickCommand => CommandsBackend.Instance.OpenFolderInCurrentViewCommand;
    }
}