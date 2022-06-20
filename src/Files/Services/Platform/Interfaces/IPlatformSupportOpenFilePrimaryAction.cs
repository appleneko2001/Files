namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportOpenFilePrimaryAction : IPlatformSupportFileActionBase
    {
        void LetPlatformHandleThisFile(string path);
    }
}