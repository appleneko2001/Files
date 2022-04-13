namespace Files.Adb.Operations.Results
{
    public class AdbOperationResult
    {
        public double Progress { get; set; }
        public object Result { get; set; }
    }
    
    public class AdbOperationResult<T> : AdbOperationResult
    {
        public new T Result { get; set; }
    }
}