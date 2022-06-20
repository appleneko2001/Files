namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportFileActionBase
    {
        bool CanHandleThisFile(string path);
    }
}