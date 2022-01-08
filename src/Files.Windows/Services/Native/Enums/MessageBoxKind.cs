using System;

namespace Files.Windows.Services.Native.Enums
{
    [Flags]
    public enum MessageBoxKind : long
    {
        Default = 0x00000000L,
        
        IconStop = 0x00000010L,
        
        SystemModal = 0x00001000L
    }
}