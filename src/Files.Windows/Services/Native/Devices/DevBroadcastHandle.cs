using System;
using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Devices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastHandle
    {
        public int Size;
        public WinDeviceKind DeviceType;
        private uint dbch_reserved;
        public IntPtr Handle;
        public IntPtr dbch_hdevnotify;
        
        public Guid dbch_eventguid;
        public long dbch_nameoffset;
        public byte dbch_data;
        public byte dbch_data1;
    }
}