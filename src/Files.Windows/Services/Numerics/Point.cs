using System.Runtime.InteropServices;

namespace Files.Windows.Services.Numerics
{
    [StructLayout(LayoutKind.Sequential)]  
    public struct Point
    {
        public int x;
        public int y;

        public Point(int v)
        {
            x = v;
            y = v;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"[{x.ToString()}, {y.ToString()}]";
        }
    }
}