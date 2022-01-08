using System;
using System.Runtime.InteropServices;
using System.Text;
using Files.Windows.Services.Native.Shell;
using Files.Windows.Services.Native.Shell.Enums;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IdentifierTypo

namespace Files.Windows.Services.Native
{
    public static partial class NativeApi
    {
        public const int CSIDL_DRIVES = 0x0011;
        public const int CSIDL_DESKTOP = 0x0000;
        
        [DllImport(WinShell)]
        public static extern uint SHGetSpecialFolderLocation(IntPtr hWnd, int csidl, [Out] out ItemIdList ppidl);
        
        [DllImport(WinShell)]
        public static extern uint SHGetSpecialFolderLocation(IntPtr hWnd, int csidl, ref IntPtr ppidl);

        [DllImport(WinShell, CharSet=CharSet.Auto)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);
        
        
        [DllImport(WinShell, EntryPoint="#2", CharSet=CharSet.Auto)]
        public static extern ulong SHChangeNotifyRegister([In]IntPtr hWnd, ShellChangeNotifyEvent fSources, ShellChangeNotifyEventKind fEvents, uint wMsg, int cEntries, [MarshalAs(UnmanagedType.LPArray)] ref SHChangeNotifyEntry[] pshcne);

        [DllImport(WinShell, EntryPoint="#4", CharSet=CharSet.Auto)]
        public static extern bool SHChangeNotifyDeregister(ulong ulId);
    }
}