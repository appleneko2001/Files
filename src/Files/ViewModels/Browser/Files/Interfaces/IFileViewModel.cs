using Files.ViewModels.Browser.Preview;

namespace Files.ViewModels.Browser.Files.Interfaces
{
    public interface IFileViewModel : IFileSystemViewModel
    {
        bool IsPreviewReady { get; }
        
        PreviewableViewModelBase? Preview { get; }
        
        IconViewModelBase Icon { get; }
    }
}