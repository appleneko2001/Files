using System;
using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Shell
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHChangeNotifyEntry
    {
        public IntPtr pidl;
        public bool fRecursive;
    }
}