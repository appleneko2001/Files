using System.Threading;
using Files.Models.Android.Storages;

namespace Files.ViewModels.Browser.Files.Android
{
    public class AdbFileItemViewModel : AdbFileSystemItemViewModel
    {
        public AdbFileItemViewModel(AdbBrowserContentViewModel parent, AdbListFilesItemModel model) 
            : base(parent, model)
        {
            Size = model.Size;
        }

        public override bool IsFolder => false;

        public override void PreAction(CancellationToken token)
        {
            // Pull file from device first
            
        }
    }
}