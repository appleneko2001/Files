using System;
using Files.Services.Watchers;

namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportDeviceWatcherService
    {
        void Start();
        void Stop();
        event EventHandler<DeviceChangedEventArgs> OnDeviceChanged;
    }
}