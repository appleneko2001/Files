namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportExecuteApplication
    {
        void NativeExecuteApplication(string path);
        
        bool IsExecutableApplication(string path);
    }
}