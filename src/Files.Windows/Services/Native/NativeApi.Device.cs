// Deprecated, no longer to use this one.
/*
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Files.Windows.Services.Native.Devices;

namespace Files.Windows.Services.Native
{
    public static partial class NativeApi
    {
        internal static readonly Guid GuidDevInterfaceUsbDevice = Guid.Parse("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
        private static readonly Guid GuidDevInterfaceDisk = Guid.Parse("53F56307-B6BF-11D0-94F2-00A0C91EFB8B"); // Disk
        private static readonly Guid GuidIoVolumeDeviceInterface = Guid.Parse("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        
        private static IntPtr usbNotificationHandle, volumeNotificationHandle;
        
        
        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle">Handle to the window receiving notifications.</param>
        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            var dbi = new DevBroadcastDeviceInterface
            {
                DeviceType = WinDeviceKind.DeviceInterface,
                Reserved = 0,
                ClassGuid = GuidDevInterfaceDisk,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            var buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            usbNotificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
            if (usbNotificationHandle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        
        public static void RegisterVolumeNotification(IntPtr windowHandle)
        {
            var dbi = new DevBroadcastDeviceInterface
            {
                DeviceType = WinDeviceKind.DeviceInterface,
                Reserved = 0,
                ClassGuid = GuidIoVolumeDeviceInterface,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            var buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            volumeNotificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
            if (volumeNotificationHandle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Unregisters the window for USB device notifications
        /// </summary>
        public static void UnregisterUsbDeviceNotification()
        {
            if(usbNotificationHandle != IntPtr.Zero)
                UnregisterDeviceNotification(usbNotificationHandle);
        }
        
        public static void UnregisterVolumeNotification()
        {
            if(usbNotificationHandle != IntPtr.Zero)
                UnregisterDeviceNotification(volumeNotificationHandle);
        }
        
        [DllImport(WinUser, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport(WinUser)]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceInterface
        {
            internal int Size;
            internal WinDeviceKind DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }
    }
}
*/