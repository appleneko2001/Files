namespace Files.Windows.Services.Native
{
    /// <summary>
    /// Stripped index of windows message kind. Used for execute some features from native api. Dont abuse it:)
    /// </summary>
    public enum MessageDefinitions : uint
    {
        Null = 0x0000,
        Create = 0x0001,
        WindowDestroyed = 0x0002,
        Activate = 0x0006,
        Close = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        
        /// <summary>
        /// WM_SYSCOLORCHANGE
        /// </summary>
        ThemeChanged = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001a,
        WM_DEVMODECHANGE = 0x001b,
        /// <summary>
        /// WM_ACTIVATEAPP
        /// </summary>
        WM_ACTIVATEAPP = 0x001c,
        WM_FONTCHANGE = 0x001d,
        WM_TIMECHANGE = 0x001e,
        WM_GETMINMAXINFO = 0x0024,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003d,
        /// <summary>
        /// Windows requesting to compact RAM usage to all toplevel window.
        /// </summary>
        CompactMemoryUsage = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        
        /// <summary>
        /// Notifies applications that the system, typically a battery-powered personal computer, is about to enter a suspended mode.
        /// </summary>
        WM_POWER = 0x0048,
        WM_COPYGLOBALDATA = 0x0049,
        WM_COPYDATA = 0x004a,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_SYSTIMER = 0x0118,
        WM_ENTERIDLE = 0x0121,
        
        //Notifies applications that a power-management event has occurred.
        WM_POWERBROADCAST = 0x0218,
        DeviceChanged = 0x0219,
        User = 0x0400,
        
        UserMediaChanged = User + 1,
        UserNullMessage = User + 512,
        
        Application = 0x8000,
    }
}