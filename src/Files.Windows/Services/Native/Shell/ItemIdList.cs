using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Shell
{
    /// <summary>
    /// Contains a list of item identifiers. Source from pinvoke.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct ItemIdList
    {
        /// <summary>
        /// A list of item identifiers.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)] 
        public ShellItemId mkid;
    }
}