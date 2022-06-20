using System.Collections.Generic;
using System.Threading;
using Files.Operations.Interfaces;

namespace Files.Operations.Android
{
    public class AdbSaveToLocalStorageOperation : IOperation
    {
        public IAsyncEnumerable<OperationStatusModel> RunAsync(IDictionary<string, object> args, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}