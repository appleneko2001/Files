namespace Files.Windows.Services.Native.Shell.Enums
{
    public enum ShellChangeNotifyEventKind : long
    {
        MediaInserted = 0x00000020L,
        MediaRemoved = 0x00000040L,
        
        DriveRemoved = 0x00000080L,
        DriveAdded = 0x00000100L,
    }
}