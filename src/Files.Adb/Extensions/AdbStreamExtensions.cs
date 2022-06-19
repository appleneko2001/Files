using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Files.Adb.Apis;
using Files.Adb.Builders;
using Files.Adb.Models;

namespace Files.Adb.Extensions
{
    public static class AdbStreamExtensions
    {
        internal static Encoding Encoding = Encoding.UTF8;
        
        public static AdbStream GetVersion(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.Version);
            
            return stream;
        }

        public static AdbStream GetFeatures(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.Features);

            return stream;
        }
        
        public static AdbStream Shell(this AdbStream stream, string command)
        {
            SingleCommand(stream, AdbRequestStrings.Shell
                .AppendParam("command", command)
                .AppendParam("args", ",raw"));
            
            return stream;
        }

        public static AdbStream Execute(this AdbStream stream, string cmd)
        {
            SingleCommand(stream, AdbRequestStrings.Exec
                .AppendParam("command", cmd));

            return stream;
        }
        
        /// <summary>
        /// Set target device for the instance of ADB stream.
        /// </summary>
        public static AdbStream SetDevice(this AdbStream stream, AdbConnection target)
        {
            ExtractDeviceAndAdbHost(target.GetConnectionUri(), out var device, out var adb);
            SingleCommand(stream, $"host:transport:{device}");

            return stream;
        }
        
        /// <summary>
        /// Send get devices command to adb server.
        /// </summary>
        public static AdbStream GetDevices(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.GetDevicesList);
            
            return stream;
        }

        /// <summary>
        /// Send get devices command to adb server.
        /// </summary>
        public static AdbStream GetDevicesFullList(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.GetDevicesFullList);
            
            return stream;
        }

        public static AdbStream TrackDevices(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.TrackDevices);

            return stream;
        }
        
        public static int? ToNullableInt(this AdbStream stream)
        {
            using (var reader = ToStreamReader(stream))
            {
                if (int.TryParse(reader.ReadLine(), out var result))
                    return result;

                return null;
            }
        }
        
        public static string ToString(this AdbStream stream)
        {
            using (var reader = ToStreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Processes the stream and returns a list of devices.
        /// </summary>
        public static IReadOnlyList<AdbDeviceModel> ToDevicesList(this AdbStream stream)=>
            ProcessAndToList(stream, AdbDeviceModel.Parse);

        public static IEnumerable<string> ReceiveLinesContinuous(this AdbStream stream)
        {
            var baseStream = stream.GetStream();
            
            while (baseStream.CanRead)
            {
                yield return ReadLineCoreAsync(baseStream).Result;
            }
        }

        /// <summary>
        /// Processes the stream and returns the result as a list of the specified type.
        /// </summary>
        /// <param name="stream">Current ADB stream instance.</param>
        /// <param name="parser">Parser for specified type.</param>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <returns>Returns a read-only list of specified type.</returns>
        public static IReadOnlyList<T> ProcessAndToList<T>(this AdbStream stream, Func<string, T> parser, bool skipSize = false)
        {
            var result = new List<T>();
            using (var reader = ToStreamReader(stream, skipSize))
            {
                string? line;

                while ((line = reader?.ReadLine()) != null)
                {
                    result.Add(parser(line));
                }
            }
            
            return result;
        }

        public static async IAsyncEnumerable<string> ToEnumerableAsync(this AdbStream stream, bool skipSize = false)
        {
            using var reader = ToStreamReader(stream, skipSize);
            string? line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }

        public static string ReadString(this AdbStream stream)
        {
            var baseStream = stream.GetStream();
            
            return ReadLineCoreAsync(baseStream).Result;
        }
        
        public static async Task<string> ReadStringAsync(this AdbStream stream)
        {
            var baseStream = stream.GetStream();
            
            return await ReadLineCoreAsync(baseStream);
        }

        public static StreamReader ToStreamReader(this AdbStream stream, bool skipSize = false)
        {
            var baseStream = stream.GetStream();

            if (skipSize)
                return new StreamReader(baseStream, Encoding);
            
            var lenHex = new byte[4];
            
            baseStream.Read(lenHex);

            var lenStr = Encoding.ASCII.GetString(lenHex);
            var size = int.Parse(lenStr, NumberStyles.HexNumber);

            return new StreamReader(baseStream, Encoding);
        }
        
        public static AdbSyncBuilder UseSyncFeature(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.Sync);
            
            return new AdbSyncBuilder(stream);
        }

        private static void SingleCommand(AdbStream stream, string command)
        {
            stream.Send(command);
            if (!HandleResult(stream, out var reason))
            {
                throw new Exception(reason);
            }
        }

        /// <summary>
        /// Extract device serial (or IP address and port) and target ADB host from URI
        /// </summary>
        public static void ExtractDeviceAndAdbHost(this Uri uri, out string device, out string host)
        {
            device = uri.UserInfo;
            host = uri.Host;
        }

        public static AdbConnection ExtractAdbConnectionModelFromUri(this Uri uri)
        {
            return new AdbConnection(uri.UserInfo, uri.Host);
        }

        private static bool HandleResult(AdbStream stream, out string? reason)
        {
            var r = stream.GetResult();
            reason = string.Empty;
            
            switch (r)
            {
                case "OKAY":
                    return true;
                
                case "FAIL":
                    reason = ReadString(stream);
                    return false;
                
                default:
                    return false;
            }
        }

        private static async Task<string> ReadLineCoreAsync(this Stream stream)
        {
            var lenHex = new byte[4];
            
            await stream.ReadAsync(lenHex);

            var lenStr = Encoding.ASCII.GetString(lenHex);
            var size = int.Parse(lenStr, NumberStyles.HexNumber);
            
            var buffer = new byte[size];
            await stream.ReadAsync(buffer);
            
            return Encoding.GetString(buffer);
        }
    }
}