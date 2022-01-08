using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Files.Models.Devices;
using Files.Services;
using Files.Windows.Models;
using Files.Windows.Services.Native;
using Files.Windows.Services.Native.Enums;

namespace Files.Windows.Services
{
    public class WindowsApiBridge : PlatformSpecificBridge
    {
        private Collection<DeviceModel> _devices;
        private MyComputerDeviceModel _myComputerModel;
        public WindowsApiBridge()
        {
            _myComputerModel = new MyComputerDeviceModel();
            
            _devices = new Collection<DeviceModel>
            {
                _myComputerModel
            };
        }
        
        public override void PopupMessageWindow(string title, string content)
        {
            NativeApi.MessageBox(IntPtr.Zero, content, title, (long) MessageBoxKind.Default);
        }

        /// <summary>
        /// Get all storage entries, including removable storage.
        /// </summary>
        /// <returns>Return all available storages.</returns>
        public override IReadOnlyCollection<DeviceModel> GetDeviceEntries()
        {
            return _devices;
        }
    }
}