using Avalonia;

namespace Files.Extensions
{
    public static class PixelSizeExtensions
    {
        public static bool GreaterThan(this PixelSize a, PixelSize b)
        {
            return a.Width > b.Width && a.Height > b.Height;
        }
        
        public static bool GreaterEqualThan(this PixelSize a, PixelSize b)
        {
            return a.Width >= b.Width && a.Height >= b.Height;
        }
        
        public static bool LessThan(this PixelSize a, PixelSize b)
        {
            return a.Width < b.Width && a.Height < b.Height;
        }
        
        public static bool LessEqualThan(this PixelSize a, PixelSize b)
        {
            return a.Width <= b.Width && a.Height <= b.Height;
        }
    }
}