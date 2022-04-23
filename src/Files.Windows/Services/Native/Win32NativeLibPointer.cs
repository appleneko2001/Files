using System;
using Files.Services.Platform;

namespace Files.Windows.Services.Native
{
    public class Win32NativeLibPointer : NativeResourcePointer
    {
        public Win32NativeLibPointer(IntPtr ptr) : base(ptr)
        {
        }

        protected override void ReleasePointer(IntPtr ptr) => NativeApi.FreeLibrary(ptr);
    }
}