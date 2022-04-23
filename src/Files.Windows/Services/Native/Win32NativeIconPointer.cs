using System;
using Files.Services.Platform;
using static Files.Windows.Services.Native.NativeApi;

namespace Files.Windows.Services.Native
{
    // Used for ExtractIconEx
    public class Win32NativeIconPointer : NativeResourcePointer
    {
        public Win32NativeIconPointer(IntPtr ptr) : base(ptr)
        {
        }

        protected override void ReleasePointer(IntPtr ptr) => DestroyIcon(ptr);
    }
}