using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Devices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastHdr
    {
        public uint Size;
        public WinDeviceKind DeviceType;
        private uint dbch_reserved;
    }
}