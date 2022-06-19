namespace Files.Adb.Commands.Sync
{
    public class AdbSyncCommandBase
    {
        public const string Done = "DONE";
        public const string Fail = "FAIL";
        public const string Dent = "DENT";
        
        public AdbStream AdbStream { get; set; }
    }
}