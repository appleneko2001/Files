using System;
using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native
{
    public static partial class NativeApi
    {
        [DllImport(WinKernel, CharSet = CharSet.Auto)]
        public static extern int GetCurrentThreadId();
        
        [DllImport(WinKernel, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
            IntPtr lpInBuffer, uint nInBufferSize,
            IntPtr lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport(WinKernel, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryEx(string lpLibFileName, IntPtr hFile, int dwFlags);
        
        [DllImport(WinKernel, CharSet = CharSet.Auto)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }
}