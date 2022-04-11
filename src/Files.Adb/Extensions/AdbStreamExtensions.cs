using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Files.Adb.Apis;
using Files.Adb.Models;
using Files.Adb.Models.Connections;

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
        
        public static AdbStream Shell(this AdbStream stream, string command)
        {
            SingleCommand(stream, AdbRequestStrings.Shell.AppendParam("command", command).AppendParam("args", ",raw"));
            
            return stream;
        }
        
        /// <summary>
        /// Set target device for the instance of ADB stream.
        /// </summary>
        public static AdbStream SetDevice(this AdbStream stream, IAdbConnection target)
        {
            SingleCommand(stream, $"host:transport:{target.GetAdbConnectionString()}");

            return stream;
        }

        /// <summary>
        /// Send get devices command to adb server.
        /// </summary>
        public static AdbStream GetDevices(this AdbStream stream)
        {
            SingleCommand(stream, AdbRequestStrings.GetDevicesFullList);
            
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

        public static IReadOnlyList<string> ToList(this AdbStream stream, bool skipSize = false)
        {
            var result = new List<string>();
            using (var reader = ToStreamReader(stream, skipSize))
            {
                string? line;

                while ((line = reader?.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }

            return result;
        }

        public static string ReadString(this AdbStream stream)
        {
            var baseStream = stream.GetStream();
            
            var lenHex = new byte[4];
            
            baseStream.Read(lenHex);

            var lenStr = Encoding.ASCII.GetString(lenHex);
            var size = int.Parse(lenStr, NumberStyles.HexNumber);
            
            var buffer = new byte[size];
            baseStream.Read(buffer);
            
            return Encoding.GetString(buffer);
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

        private static void SingleCommand(AdbStream stream, string command)
        {
            stream.Send(command);
            if (!HandleResult(stream, out var reason))
            {
                throw new Exception(reason);
            }
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
    }
}