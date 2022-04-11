using System;

namespace Files.Adb.Models.Connections
{
    public class AdbNormalConnection : IAdbConnection
    {
        private string _serial;
        public string Serial => _serial;

        public AdbNormalConnection(string serial)
        {
            _serial = serial.ToUpperInvariant();
        }

        public Uri GetConnectionUri()
        {
            return new Uri($"adbfile://{_serial}");
        }

        public string GetAdbConnectionString() => _serial;
    }
}