using System;
using System.Collections.Generic;
using System.Threading;
using Files.Adb.Commands.Sync;

namespace Files.Adb.Builders
{
    public class AdbSyncBuilder
    {
        public AdbStream AdbStream { get; }

        public IDictionary<string, object> Parameters { get; }

        public AdbSyncBuilder(AdbStream adbStream)
        {
            AdbStream = adbStream;
            Parameters = new Dictionary<string, object>();
        }

        public AdbSyncBuilder<TResult> UseCommand<TResult>(IAdbSyncCommand<TResult> operation) 
            where TResult : AdbSyncCommandResultBase
        {
            return new AdbSyncBuilder<TResult>(this, operation);
        }
    }

    public class AdbSyncBuilder<TResult> where TResult : AdbSyncCommandResultBase
    {
        private readonly AdbSyncBuilder _inherit;
        private readonly IAdbSyncCommand<TResult> _operation;

        public AdbSyncBuilder(AdbSyncBuilder inherit, IAdbSyncCommand<TResult> operation)
        {
            _inherit = inherit;
            _operation = operation;
        }
        
        public AdbSyncBuilder<TResult> WithParameter(string k, object v)
        {
            _inherit.Parameters.Add(k, v);

            return this;
        }

        public IAsyncEnumerable<TResult> Execute(CancellationToken cancellationToken = default)
        {
            if(_operation == null)
                throw new ArgumentNullException(nameof(_operation));
            
            _operation.AdbStream = _inherit.AdbStream;
            
            return _operation.RunAsync(_inherit.Parameters, cancellationToken);
        }
    }
}