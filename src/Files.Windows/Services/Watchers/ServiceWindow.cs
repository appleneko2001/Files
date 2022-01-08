using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Files.Windows.Services.Native;

using static Files.Windows.Services.Native.NativeApi;

namespace Files.Windows.Services.Watchers
{
    public class ServiceWindow : IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WndProc _msgHandlerDelegate;
        private readonly HandleRef _handleWindow;
        public IntPtr GetHandle() => _handleWindow.Handle;

        internal Func<MessageDefinitions, IntPtr, IntPtr, IntPtr?> DeviceWatcherHandler;
        
        public ServiceWindow(IntPtr instance)
        {
            // Remove new NativeApi.WndProc() will cause exception after a while. 
            // ReSharper disable once RedundantDelegateCreation
            _msgHandlerDelegate = new WndProc(MessageHandler);
            
            var wndClassInst = new WindowClass
            {
                hInstance = instance,
                lpfnWndProc = _msgHandlerDelegate,
                lpszClassName = "FilesAvaloniaApplicationService"
            };
            
            var windowClass = RegisterClass(ref wndClassInst);
            
            // So that's why I cant use message-only window here.
            // How odd
            // https://stackoverflow.com/questions/10168467/catching-the-wm-devicechange
            
            _handleWindow = new HandleRef(this, CreateWindowEx(0x08000000, new IntPtr((int)(uint)windowClass),
                "FilesAvaloniaApplication-MessageHandler", 0, 0
                , 0, 0, 0, IntPtr.Zero, IntPtr.Zero, instance, IntPtr.Zero));
        }

        internal bool PullEvent(CancellationToken cancellationToken)
        {
            while (PeekMessage(out var message, IntPtr.Zero, 0, 0, 0x0001) > 0)
            {
                if (message.Msg == (uint) MessageDefinitions.WM_QUIT)
                    return false;
                
                DispatchMessage(ref message);
            }

            if (cancellationToken.IsCancellationRequested)
                return false;

            if (WaitMessage() == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return true;
        }

        /// <summary>
        /// Process message data
        /// </summary>
        private IntPtr MessageHandler(IntPtr hWnd, MessageDefinitions msg, IntPtr wParam, IntPtr lParam)
        {
            //Console.WriteLine(msg.ToString());
            switch (msg)
            {
                case MessageDefinitions.UserNullMessage:
                    return IntPtr.Zero + 1;
                
                case MessageDefinitions.UserMediaChanged:
                case MessageDefinitions.DeviceChanged:
                {
                    var result = DeviceWatcherHandler(msg, wParam, lParam);
                    if (result == null)
                        break;
                    
                    return result.Value;
                }
            }

            return (IntPtr)DefWindowProc(hWnd, (uint)msg, wParam, lParam);
        }
        
        private void ReleaseUnmanagedResources()
        {
            DestroyWindow(_handleWindow.Handle);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ServiceWindow()
        {
            ReleaseUnmanagedResources();
        }
    }
}