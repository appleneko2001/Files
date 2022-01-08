namespace Files.Windows.Services.Native.Shell.Enums
{
    public enum ShellChangeNotifyEvent : long
    {
        InterruptLevel = 0x1,
        ShellLevel = 0x2,
        RecursiveInterrupt = 0x1000,
        NewDelivery = 0x8000,
    }
}