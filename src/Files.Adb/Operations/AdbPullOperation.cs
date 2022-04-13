using System;
using System.Collections.Generic;
using System.Threading;
using Files.Adb.Operations.Results;

namespace Files.Adb.Operations
{
    public class AdbPullOperation : AdbOperationBase, IAdbOperation
    {
        public async IAsyncEnumerable<AdbOperationResult> RunAsync(IDictionary<string, object> args,
            CancellationToken cancellationToken)
        {
            if (AdbStream == null)
                throw new ArgumentNullException(nameof(AdbStream));
            
            var localPath = (string)args["localPath"];
            var remotePath = (string)args["remotePath"];
            
            yield break;
        }
    }
}