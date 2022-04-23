using System.Windows.Input;
using Material.Icons;

namespace Files.ViewModels.Browser.Files.Interfaces
{
    public interface IFileSystemViewModel
    {
        MaterialIconKind? AdditionalIconKind { get; }
        
        string Name { get; }
        string DisplayName { get; }
        string FullPath { get; }
        long? Size { get; }
        bool IsVisible { get; }
        
        bool IsReadonly { get; }

        ICommand? OnClickCommand { get; }
    }
}