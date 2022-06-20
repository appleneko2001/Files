using System.Collections.Generic;
using Files.Models.Devices;

namespace Files.Services.Platform.Interfaces
{
    public interface IPlatformSupportDeviceEntries
    {
        IReadOnlyCollection<DeviceModel> GetDeviceEntries();
    }
}