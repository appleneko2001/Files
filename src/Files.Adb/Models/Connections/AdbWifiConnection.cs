using System;

namespace Files.Adb.Models.Connections
{
    public class AdbWifiConnection : IAdbConnection
    {
        private readonly string _ipAddress;
        private readonly int _port;

        public AdbWifiConnection(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }
        
        public Uri GetConnectionUri()
        {
            return new Uri($"adbfile://{_ipAddress}:{_port}");
        }

        public string IpAddress => _ipAddress;

        public int Port => _port;

        public string GetAdbConnectionString() => $"{_ipAddress}:{_port}";
    }
}