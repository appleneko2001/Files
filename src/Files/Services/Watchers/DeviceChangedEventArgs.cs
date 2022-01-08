using System;
using System.IO;
using Files.Models.Devices;
using Files.Services.Watchers.Enums;

namespace Files.Services.Watchers
{
    public class DeviceChangedEventArgs : EventArgs
    {
        public DeviceStatus Action;
        public bool IsConnectedAbnormally;
        public bool IsWorkAbnormally;

        public string Entry;
        public DriveInfo DriveInfo;
    }
}