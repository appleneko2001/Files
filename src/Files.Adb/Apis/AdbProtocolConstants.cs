namespace Files.Adb.Apis
{
    public class AdbProtocolConstants
    {
        // Official ADB will receive and send data packet with buffer size header.
        // by default it should be 4, that means buffer size is Int32.
        public const int AdbProtocolSizeHeader = 4;

        // Character length should match AdbProtocolSizeHeader
        public const string AdbNumericToHexFormat = "X4";
    }
}