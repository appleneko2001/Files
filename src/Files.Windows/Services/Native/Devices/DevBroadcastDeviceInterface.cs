using System;
using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Devices
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DevBroadcastDeviceInterface
    {
        public uint Size;
        public WinDeviceKind DeviceType;
        private uint dbch_reserved;
        public Guid ClassGuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=255)]
        public string Name;

        public override string ToString()
        {
            return $"{DeviceType.ToString()} {Name}";
        }
    }
}