using System;

namespace Files.Services.Platform
{
    public abstract class NativeResourcePointer : IDisposable
    {
        private IntPtr _ptr;

        public IntPtr Pointer
        {
            get => _ptr;
            protected set => _ptr = value;
        }

        public NativeResourcePointer(IntPtr ptr)
        {
            _ptr = ptr;
        }

        protected abstract void ReleasePointer(IntPtr ptr);
        
        public void Dispose()
        {
            if (Pointer == IntPtr.Zero)
                return;

            var ptr = Pointer;
            Pointer = IntPtr.Zero;

            ReleasePointer(ptr);
        }
    }
}