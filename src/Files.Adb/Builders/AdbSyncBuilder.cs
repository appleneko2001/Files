using System.Collections.Generic;
using System.Threading;
using Files.Adb.Operations;
using Files.Adb.Operations.Results;

namespace Files.Adb.Builders
{
    public class AdbSyncBuilder
    {
        private IAdbOperation _operation;
        private AdbStream _adbStream;
        private IDictionary<string, object> parameters;

        public AdbSyncBuilder(AdbStream adbStream)
        {
            _adbStream = adbStream;
            parameters = new Dictionary<string, object>();
        }

        public AdbSyncBuilder UseOperation(IAdbOperation operation)
        {
            _operation = operation;

            return this;
        }

        public AdbSyncBuilder WithParameter(string k, object v)
        {
            parameters.Add(k, v);

            return this;
        }

        public IAsyncEnumerable<AdbOperationResult> PerformOperation(CancellationToken cancellationToken = default)
        {
            _operation.AdbStream = _adbStream;
            
            return _operation.RunAsync(parameters, cancellationToken);
        }
    }
}