using Files.Services.Watchers;

namespace Files.Services
{
    public class OperationSystemService
    {
        public virtual DeviceWatcher DeviceWatcher { get; }
        
        public virtual PlatformSpecificBridge? ApiBridge { get; }

        public virtual void Stop()
        {
            
        }
    }
}