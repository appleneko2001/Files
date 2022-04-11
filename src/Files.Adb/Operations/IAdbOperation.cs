using System.Collections.Generic;

namespace Files.Adb.Operations
{
    public interface IAdbOperation
    {
        void Run(IDictionary<string, object> args);
    }
}