using System;

namespace Files.Services.Watchers
{
    public abstract class DeviceWatcher
    {
        public abstract void Start();

        public abstract void Stop();
        
        public abstract event EventHandler<DeviceChangedEventArgs> OnDeviceChanged;
    }
}