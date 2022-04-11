using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Files.Windows.Services.Native.Devices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastPort
    {
        public uint Size;
        public WinDeviceKind DeviceType;
        private uint dbch_reserved;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] dbch_name;
        
        public string? DeviceName => new(dbch_name);
    }
}