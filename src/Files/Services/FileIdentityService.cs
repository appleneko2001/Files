using System.IO;
using FileSignatures;
using FileSignatures.Formats;

namespace Files.Services
{
    public class FileIdentityService
    {
        private static FileFormatInspector _inspectorInstance;
        
        static FileIdentityService()
        {
            _inspectorInstance = new FileFormatInspector();
        }
        
        public static bool DetermineFilePreviewable(FileInfo fi)
        {
            using (var stream = fi.OpenRead())
            {
                return DetermineFilePreviewable(stream);
            }
        }
        
        public static bool DetermineFilePreviewable(Stream stream)
        {
            var format = _inspectorInstance.DetermineFileFormat(stream);

            switch (format)
            {
                case Png:
                case JpegJfif:
                case JpegExif:
                case Jpeg:
                case Gif:
                case Bmp:
                case Image:
                    return true;
            }

            return false;
        }
    }
}