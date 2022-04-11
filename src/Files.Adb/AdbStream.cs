// Inspired by https://github.com/quamotion/madb

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Files.Adb
{
    public class AdbStream : IDisposable
    {
        private readonly Encoding _encoding;
        private readonly Socket _socket;
        public Socket Socket => _socket;

        private int _adbVersion;
        
        public AdbStream(Encoding encoding)
        {
            _encoding = encoding;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }
        
        public bool Connect(IPAddress ip, int port, int timeoutMs = 30000)
        {
            _socket.Connect(ip, port);

            return _socket.Connected;
        }

        public bool ConnectAsync(IPAddress ip, int port, int timeoutMs = 30000)
        {
            var ar = _socket.BeginConnect(ip, port, ConnectCallback, this);
            if (!ar.AsyncWaitHandle.WaitOne(timeoutMs))
            {
                throw new TimeoutException("Waiting too long and no answer :(");
            }

            return ar.IsCompleted && _socket.Connected;
        }

        public int Send(string request)
        {
            // Append length bytes to the start of the data bytes
            
            var data = _encoding.GetBytes(request);
            var length = data.Length;
            var lengthBytes = Encoding.ASCII.GetBytes(length.ToString("X4"));
            var lengthBytesCount = lengthBytes.Length;
            var lengthBytesOffset = 0;
            
            var buffer = new byte[length + lengthBytesCount];
            Array.Copy(lengthBytes, 0, buffer, lengthBytesOffset, lengthBytesCount);
            Array.Copy(data, 0, buffer, lengthBytesCount, length);
            
            return _socket.Send(buffer);
        }

        public string GetResult() => GetResponseCore(4);

        public Stream GetStream() => new NetworkStream(_socket);

        private byte[] GetBytes(int len)
        {
            var buf = new byte[len];
            var receivedLen = _socket.Receive(buf);
            
            if(receivedLen != buf.Length)
                throw new Exception();

            return buf;
        }
        
        private string GetResponseCore(int len) => _encoding.GetString(GetBytes(len));

        public void Dispose()
        {
            _socket.Dispose();
        }
        
        private static void ConnectCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is not AdbStream connection)
                throw new NullReferenceException("Unable to get AdbConnection.");
            
            connection.Socket.EndConnect(ar);
        }
    }
}