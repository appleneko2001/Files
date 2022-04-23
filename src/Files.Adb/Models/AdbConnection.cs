using System;

namespace Files.Adb.Models
{
    public class AdbConnection
    {
        private string _device;
        public string Device => _device;

        private string _adbHost;
        public string AdbHost => _adbHost;

        public AdbConnection(string device, string? adbHost = null)
        {
            _device = device;
            _adbHost = adbHost ?? "127.0.0.1";
        }

        public Uri GetConnectionUri()
        {
            // standard sucks sometimes
            // but there would another way to solve it
            
            return new Uri($"adbfile://{GetAdbConnectionString()}@{AdbHost}");
        }

        public string GetAdbConnectionString() => _device;

        public override string ToString() => $"{GetAdbConnectionString()}";

        public override bool Equals(object obj)
        {
            return obj is AdbConnection c && Equals(c);
        }

        protected bool Equals(AdbConnection other)
        {
            return _device == other._device && _adbHost == other._adbHost;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_device, _adbHost);
        }
    }
}