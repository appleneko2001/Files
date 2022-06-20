using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Files.Services.Platform.Interfaces;
using Files.Services.Watchers;
using Files.Services.Watchers.Enums;
using Files.Windows.Services.Native;
using Files.Windows.Services.Native.Devices;
using Files.Windows.Services.Native.Shell;
using Files.Windows.Services.Native.Shell.Enums;
// ReSharper disable IdentifierTypo

namespace Files.Windows.Services.Watchers
{
    public class WindowsDeviceWatcher : IPlatformSupportDeviceWatcherService
    {
        private const string DeviceLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public WindowsDeviceWatcher(ServiceWindow hwnd)
        {
            hwnd.DeviceWatcherHandler = DeviceWatcherHandler;
        }
        
        public void Start()
        {
            /*
            try
            {
                //NativeApi.RegisterUsbDeviceNotification(service.GetHandle());
                //NativeApi.RegisterVolumeNotification(service.GetHandle());

                return;
                var ppidl = IntPtr.Zero;
                if (NativeApi.SHGetSpecialFolderLocation(service.GetHandle(), NativeApi.CSIDL_DESKTOP, ref ppidl) == 0)
                {
                    var entry = new[]
                    {
                        new SHChangeNotifyEntry
                        {
                            fRecursive = false,
                            pidl = ppidl
                        }
                    };
                    _shellNotifyId = NativeApi.SHChangeNotifyRegister(service.GetHandle(),
                        ShellChangeNotifyEvent.ShellLevel,
                        ShellChangeNotifyEventKind.DriveAdded | ShellChangeNotifyEventKind.DriveRemoved |
                        ShellChangeNotifyEventKind.MediaInserted | ShellChangeNotifyEventKind.MediaRemoved,
                        (uint) MessageDefinitions.UserMediaChanged, 1, ref entry);

                    if (_shellNotifyId == 0)
                        throw new ArgumentOutOfRangeException(nameof(_shellNotifyId), "NotifyId should not be zero.");
                }
                else
                    throw new ArgumentNullException(nameof(ppidl), "SHGetSpecialFolderLocation returned zero. It means operation is fail or declined by Windows OS.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Shell notification register failed. It affects media insert and remove notification feature.");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }*/
        }

        public void Stop()
        {
            //if(_shellNotifyId != 0)
            //    NativeApi.SHChangeNotifyDeregister(_shellNotifyId);
        }

        public event EventHandler<DeviceChangedEventArgs> OnDeviceChanged;

        // TODO: Refactor this method.
        private IntPtr? DeviceWatcherHandler(MessageDefinitions msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case MessageDefinitions.DeviceChanged:
                {
                    WinDeviceStatus status = (WinDeviceStatus) wParam;
                    if (lParam == IntPtr.Zero)
                        return null;
                    
                    var lpdb = Marshal.PtrToStructure<DevBroadcastHdr>(lParam);
                    
                    Console.WriteLine(Enum.GetName(status));

                    switch (status)
                    {
                        case WinDeviceStatus.DBT_DEVICEARRIVAL:
                        {
                            switch (lpdb.DeviceType)
                            {
                                case WinDeviceKind.DeviceInterface:
                                {
                                    var info = Marshal.PtrToStructure<DevBroadcastDeviceInterface>(lParam);

                                    Console.WriteLine(info.ToString());
                                } break;

                                case WinDeviceKind.Port:
                                {
                                    var info = Marshal.PtrToStructure<DevBroadcastPort>(lParam);
                                    
                                    Console.WriteLine(info.ToString(), info.DeviceName);
                                    
                                    OnDeviceChanged?.Invoke(this, new DeviceChangedEventArgs
                                    {
                                        Action = DeviceStatus.Connected
                                    });
                                } break;

                                case WinDeviceKind.Volume:
                                {
                                    var info = Marshal.PtrToStructure<DevBroadcastVolume>(lParam);

                                    var entry = DeviceMaskToDeviceName(info.Unitmask);

                                    Console.WriteLine($"{info.ToString()}, {entry}");
                                    
                                    OnDeviceChanged?.Invoke(this, new DeviceChangedEventArgs
                                    {
                                        Action = DeviceStatus.Connected,
                                        Entry = entry,
                                        DriveInfo = new DriveInfo(entry)
                                    });
                                } break;
                            }
                        } break;

                        case WinDeviceStatus.DBT_DEVICEREMOVECOMPLETE:
                        {
                            switch (lpdb.DeviceType)
                            {
                                case WinDeviceKind.DeviceInterface:
                                {
                                    var info = Marshal.PtrToStructure<DevBroadcastDeviceInterface>(lParam);

                                    Console.WriteLine(info.ToString());
                                } break;
                                
                                case WinDeviceKind.Port:
                                {
                                    var info = Marshal.PtrToStructure<DevBroadcastPort>(lParam);
                                    
                                    Console.WriteLine(info.ToString());

                                    OnDeviceChanged?.Invoke(this, new DeviceChangedEventArgs
                                    {
                                        Action = DeviceStatus.Disconnected
                                    });
                                } break;

                                case WinDeviceKind.Volume:
                                {
                                    var info = Marshal.PtrToStructure<DevBroadcastVolume>(lParam);

                                    var entry = DeviceMaskToDeviceName(info.Unitmask);

                                    Console.WriteLine($"{info.ToString()}, {entry}");
                                    
                                    OnDeviceChanged?.Invoke(this, new DeviceChangedEventArgs
                                    {
                                        Action = DeviceStatus.Disconnected,
                                        Entry = entry,
                                        DriveInfo = new DriveInfo(entry)
                                    });
                                } break;
                            }
                        } break;
                    }
                } break;

                // Second way -- via Shell
                case MessageDefinitions.UserMediaChanged:
                {
                    var e = Marshal.PtrToStructure<ShellNotifyStruct>(wParam);

                    var lParamCase = (long) lParam;

                    switch ((ShellChangeNotifyEventKind)lParamCase)
                    {
                        case ShellChangeNotifyEventKind.DriveAdded:
                        case ShellChangeNotifyEventKind.MediaInserted:
                        {
                            var item1 = GetPathFromPIDL(e.dwItem1);
                            if (item1 != null)
                            {
                                var driveInfo = new DriveInfo(item1);
                                
                                OnDeviceChanged?.Invoke(this, new DeviceChangedEventArgs
                                {
                                    Action = DeviceStatus.Connected,
                                });
                            }
                        } break;

                        case ShellChangeNotifyEventKind.DriveRemoved:
                        case ShellChangeNotifyEventKind.MediaRemoved:
                        {
                            var item1 = GetPathFromPIDL(e.dwItem1);
                            if (item1 != null)
                            {
                                OnDeviceChanged?.Invoke(this, new DeviceChangedEventArgs
                                {
                                    Action = DeviceStatus.Disconnected,
                                });
                            }
                        } break;
                    }
                } break;
            }
            
            return IntPtr.Zero;
        }

        private static string DeviceMaskToDeviceName(int mask)
        {
            var log = Convert.ToInt32(Math.Log(mask, 2));
            return log < DeviceLetters.Length ? $"{DeviceLetters[log].ToString()}:\\" : string.Empty;
        }
        
        private string GetPathFromPIDL(IntPtr pidl)
        {
            var strBuilder = new StringBuilder();
            
            if (pidl != IntPtr.Zero)
                // ReSharper disable once ConvertIfStatementToReturnStatement
                NativeApi.SHGetPathFromIDList(pidl, strBuilder);

            return strBuilder.ToString();
        }
    }
}