namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportShowOpenWithDialog : IPlatformSupportFileActionBase
    {
        void ShowOpenWithApplicationDialog(string path);
    }
}