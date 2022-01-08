using System;
using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Shell
{
    /// <summary>
    /// Contains and receives information for change notifications. Source from pinvoke.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ShellNotifyStruct
    {
        /// <summary>
        /// Contains the previous PIDL or name of the folder.
        /// </summary>
        public IntPtr dwItem1;

        /// <summary>
        /// Contains the new PIDL or name of the folder.
        /// </summary>
        public IntPtr dwItem2;
    }
}