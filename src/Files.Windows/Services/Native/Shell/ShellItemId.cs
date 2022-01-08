using System.Runtime.InteropServices;

namespace Files.Windows.Services.Native.Shell
{
    /// <summary>
    /// Defines an item identifier. Source from pinvoke.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct ShellItemId
    {
        /// <summary>
        /// The size of identifier, in bytes, including cb itself.
        /// </summary>
        public ushort cb;
        /// <summary>
        /// A variable-length item identifier.
        /// </summary>
        public byte[] abID;
    }
}