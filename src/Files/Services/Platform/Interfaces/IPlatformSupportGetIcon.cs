using System.IO;

namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportGetIcon
    {
        bool CanGetIconForFile(string path);
        
        NativeResourcePointer GetIconForFile(string path, int w, int h);

        Stream? GetIconStreamForFile(string path, int w, int h);
    }
}