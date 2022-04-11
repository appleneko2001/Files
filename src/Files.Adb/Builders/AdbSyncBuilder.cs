using System.Collections.Generic;

namespace Files.Adb.Builders
{
    public class AdbSyncBuilder
    {
        private IDictionary<string, object> parameters;

        public AdbSyncBuilder()
        {
            parameters = new Dictionary<string, object>();
        }
    }
}