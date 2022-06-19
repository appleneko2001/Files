using System.Collections.Generic;
using System.Threading;

namespace Files.Adb.Commands.Sync
{
    public interface IAdbSyncCommand
    {
        public AdbStream AdbStream { get; set; }
        
        IAsyncEnumerable<AdbSyncCommandResultBase> RunAsync(IDictionary<string, object> args,
            CancellationToken cancellationToken);
    }

    public interface IAdbSyncCommand<out TResult> : IAdbSyncCommand where TResult : AdbSyncCommandResultBase 
    {
        new IAsyncEnumerable<TResult> RunAsync(IDictionary<string, object> args,
            CancellationToken cancellationToken);
    }
}