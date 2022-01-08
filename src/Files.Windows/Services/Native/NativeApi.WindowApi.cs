using System;
using System.Runtime.InteropServices;
using Files.Windows.Services.Native.Enums;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IdentifierTypo

namespace Files.Windows.Services.Native
{
    public static partial class NativeApi
    {
        [StructLayout(LayoutKind.Sequential)]  
        public struct WindowClass  
        {  
            public ClassStyles style;  
            [MarshalAs(UnmanagedType.FunctionPtr)]  
            public WndProc lpfnWndProc;  
            public int cbClsExtra;  
            public int cbWndExtra;  
            public IntPtr hInstance;  
            public IntPtr hIcon;  
            public IntPtr hCursor;  
            public IntPtr hbrBackground;  
            [MarshalAs(UnmanagedType.LPTStr)]  
            public string lpszMenuName;  
            [MarshalAs(UnmanagedType.LPTStr)]  
            public string lpszClassName;  
        }
        
        [DllImport(WinUser)]  
        public static extern ushort RegisterClass([In] ref WindowClass lpWndClass);  
        
        [DllImport(WinUser)]
        public static extern void DispatchMessage([In] ref Message message);
        
        [DllImport(WinUser, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            IntPtr regResult,
            [MarshalAs(UnmanagedType.LPWStr)] string windowName, //window name
            int style,
            int x,
            int y,
            int width,
            int height,
            IntPtr parentHandle,
            IntPtr menuHandle,
            IntPtr instanceHandle,
            IntPtr additionalData);
        
        [DllImport(WinUser)]
        public static extern int DefWindowProc(
            IntPtr hwnd,
            uint msg,
            IntPtr wparam,
            IntPtr lparam);

        [DllImport(WinUser)]
        public static extern int PeekMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin,
            uint wMsgFilterMax, uint wRemoveMsg);
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(WinUser, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostThreadMessage(IntPtr threadId, MessageDefinitions msg, IntPtr wParam, IntPtr lParam);

        [DllImport(WinUser, SetLastError = true)]
        public static extern int WaitMessage();

        [DllImport(WinUser)]
        public static extern bool DestroyWindow(IntPtr hWnd);
        
        [DllImport(WinUser)]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string lpText, string lpCaption, long uType);
        
        public delegate IntPtr WndProc(IntPtr hWnd, MessageDefinitions msg, IntPtr wParam, IntPtr lParam);  
    }
}