namespace Files.Adb.Operations
{
    public class AdbOperationBase
    {
        public const string Done = "DONE";
        public const string Fail = "FAIL";
        public const string Dent = "DENT";
        
        public AdbStream AdbStream { get; set; }
    }
}