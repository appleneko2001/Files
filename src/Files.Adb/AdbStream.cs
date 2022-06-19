// Inspired by https://github.com/quamotion/madb

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using static Files.Adb.Apis.AdbProtocolConstants;

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

        public int Send(string request) =>
            SendAsync(request).Result;
        
        public int SendSyncRequest(string header, string msg) =>
            SendSyncRequestAsync(header, msg).Result;
        
        public async Task<int> SendAsync(string request)
        {
            var data = _encoding.GetBytes(request);
            
            return await SendCoreAsync(null, data);
        }
        
        public async Task<int> SendSyncRequestAsync(string header, string msg)
        {
            if(header.Length != AdbProtocolSizeHeader)
                throw new ArgumentException($"Header must be {AdbProtocolSizeHeader} characters long");
            
            var data = _encoding.GetBytes(msg);

            return await SendSyncRequestCoreAsync(header, data);
        }
        
        // send some data with 8 bytes header (command id and binary of length data)
        private async Task<int> SendSyncRequestCoreAsync(string? header, byte[] data)
        {
            var basicEnc = Encoding.ASCII;

            var collection = new List<ArraySegment<byte>>();
            
            if(header != null)
                collection.Add(basicEnc.GetBytes(header));
            
            // Get binary-style int32 data
            var len = data.Length;
            var lenBytes = BitConverter.GetBytes(len);
            
            collection.Add(lenBytes);
            
            collection.Add(data);
            
            return await _socket.SendAsync(collection, SocketFlags.None);
        }
        
        private async Task<int> SendCoreAsync(string? header, byte[] data)
        {
            var basicEnc = Encoding.ASCII;

            var collection = new List<ArraySegment<byte>>();
            
            if(header != null)
                collection.Add(basicEnc.GetBytes(header));
            
            var len = data.LongLength;
            var lenBytes = basicEnc.GetBytes(len.ToString(AdbNumericToHexFormat));
            
            collection.Add(lenBytes);
            
            collection.Add(data);
            
            return await _socket.SendAsync(collection, SocketFlags.None);
        }

        public string GetResult() => GetResponseCore(AdbProtocolSizeHeader);

        public Stream GetStream() => new NetworkStream(_socket);
        
        public Stream GetReadOnlyStream() => new NetworkStream(_socket, FileAccess.Read, true);

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