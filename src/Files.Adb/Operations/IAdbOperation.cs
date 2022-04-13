using System.Collections.Generic;
using System.Threading;
using Files.Adb.Operations.Results;

namespace Files.Adb.Operations
{
    public interface IAdbOperation
    {
        public AdbStream AdbStream { get; set; }
        
        IAsyncEnumerable<AdbOperationResult> RunAsync(IDictionary<string, object> args,
            CancellationToken cancellationToken);
    }
}