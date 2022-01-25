namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportNativeExplorer
    {
        string NativeExplorerName { get; }
        
        void OpenFolderWithNativeExplorer(string path);
    }
}