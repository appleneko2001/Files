using System.Collections.Generic;
using System.Threading;

namespace Files.Operations.Interfaces
{
    public interface IOperation
    {
        IAsyncEnumerable<OperationStatusModel> RunAsync(IDictionary<string, object> args,
            CancellationToken cancellationToken);
    }
}