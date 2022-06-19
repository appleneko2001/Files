using System;

namespace Files.Adb.Commands.Results
{
    public class AdbCommandResult
    {
        public bool IsFailed { get; set; }
        public bool IsSuccess { get; set; }
        public double Progress { get; set; }
        public Exception Exception { get; set; }
    }
    
    public class AdbCommandResult<T> : AdbCommandResult
    {
        public T Result { get; set; }
    }
}