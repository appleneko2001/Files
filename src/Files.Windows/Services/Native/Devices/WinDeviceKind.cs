// ReSharper disable EnumUnderlyingTypeIsInt

namespace Files.Windows.Services.Native.Devices
{
    public enum WinDeviceKind : int
    {
        Oem               =   0x00000000,  // oem-defined device type
        DevNode           =   0x00000001,  // devnode number
        Volume            =   0x00000002,  // logical volume
        Port              =   0x00000003,  // serial, parallel
        Network               =   0x00000004,  // network resource

        
        DeviceInterface   =   0x00000005,  // device interface class
        Handle            =   0x00000006  // file system handle
    }
}