using System;
using Files.Adb.Enums;

namespace Files.Adb.Models.Sync
{
    public class AdbFileEntry
    {
        public AdbFileEntry(byte[] stat, string path)
        {
            if (stat.Length != 12)
                throw new ArgumentException("stat must be 12 bytes long");
            
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(stat, 0, 4);
                Array.Reverse(stat, 4, 4);
                Array.Reverse(stat, 8, 4);
            }
            
            Mode = (UnixFileMode)BitConverter.ToInt32(stat, 0);
            Size = BitConverter.ToInt32(stat, 4);
            DateTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(stat, 8));
            Path = path;
        }
        
        public UnixFileMode Mode { get; }
        public long Size { get; }
        public string Path { get; }
        public DateTime DateTime { get; }
        
    }
}