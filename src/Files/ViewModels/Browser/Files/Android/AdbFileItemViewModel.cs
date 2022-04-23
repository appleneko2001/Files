using System.Threading;
using Files.Models.Android.Storages;
using Files.Services;
using Files.ViewModels.Browser.Files.Interfaces;
using Files.ViewModels.Browser.Preview;

namespace Files.ViewModels.Browser.Files.Android
{
    public class AdbFileItemViewModel : AdbFileSystemItemViewModel, IFileViewModel
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

        public bool IsPreviewReady => false;
        public PreviewableViewModelBase? Preview => null;
        public IconViewModelBase Icon => FileIdentityService.GetIconByExtension(FullPath);
    }
}