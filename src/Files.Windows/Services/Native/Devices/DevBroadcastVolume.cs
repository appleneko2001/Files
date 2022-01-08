using System;
using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Devices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastVolume
    {
        public int Size;
        public WinDeviceKind DeviceType;
        private int dbch_reserved;
        public int Unitmask;
        public DevBroadcastFlags Flags;

        public override string ToString() => $"{Enum.GetName(DeviceType)}, Unit mask: {Unitmask.ToString()}, Flags: {Enum.GetName(Flags)}";
    }
}