using System.Collections.Generic;
using Files.Models.Devices;

namespace Files.Services
{
    public abstract class PlatformSpecificBridge
    {
        /// <summary>
        /// Show a native message window with specified title and content.
        /// </summary>
        /// <param name="title">title of window</param>
        /// <param name="content">message text of window</param>
        public abstract void PopupMessageWindow(string title, string content);

        public abstract IReadOnlyCollection<DeviceModel> GetDeviceEntries();

        public abstract void NativeExecuteApplication(string path);
        
        public abstract bool IsExecutableApplication(string path);

        public abstract void LetPlatformHandleThisFile(string path);
        
        public abstract void ShowOpenWithApplicationDialog(string path);
    }
}