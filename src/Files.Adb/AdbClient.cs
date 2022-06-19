// Inspired by https://github.com/quamotion/madb

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Files.Adb.Extensions;

namespace Files.Adb
{
    public class AdbClient
    {
        private Encoding _encoding;

        public Encoding Encoding
        {
            get => _encoding;
            set
            {
                _encoding = value;
                AdbStreamExtensions.Encoding = value;
            }
        }
        
        public int Port { get; private set; }
        public IPAddress IpAddress { get; private set; }
        
        public AdbClient(IPAddress? ipAddress = null, int port = 5037)
        {
            IpAddress = ipAddress ?? IPAddress.Loopback;
            Port = port;
            
            Encoding = Encoding.UTF8;
        }

        public AdbStream CreateStream()
        {
            var connection = new AdbStream(Encoding);
            if (!connection.Connect(IpAddress, Port))
                throw new Exception();

            return connection;
        }

        public bool IsRunning()
        {
            try
            {
                using (var stream = CreateStream())
                {
                    return stream
                        .GetVersion()
                        .ToNullableInt() != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string ReadString(StreamReader reader)
        {
            var lenBuf = new char[4];
            reader.Read(lenBuf);

            var buf = new char[int.Parse(lenBuf, NumberStyles.HexNumber)];
            reader.Read(buf);

            return new string(buf);
        }
    }
}