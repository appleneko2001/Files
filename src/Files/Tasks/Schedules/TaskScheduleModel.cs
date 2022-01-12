using System;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Tasks.Schedules
{
    public class TaskScheduleModel : IDisposable
    {
        public Task Task;
        public CancellationToken CancellationToken;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Task.Dispose();
        }
    }
}