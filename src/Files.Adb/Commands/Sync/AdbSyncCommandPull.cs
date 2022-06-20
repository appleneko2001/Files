using System;
using System.Collections.Generic;
using System.Threading;

namespace Files.Adb.Commands.Sync
{
    public class AdbSyncCommandPull : AdbSyncCommandBase, IAdbSyncCommand
    {
        public async IAsyncEnumerable<AdbSyncCommandResultBase> RunAsync(IDictionary<string, object> args,
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