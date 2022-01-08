namespace Files.Windows.Services.Native.Devices
{
    public enum DevBroadcastFlags : ushort
    {
        /// <summary>
        /// Change affects media in drive. If not set, change affects physical device or drive.
        /// </summary>
        Media = 0x0001,
        
        /// <summary>
        /// Indicated logical volume is a network volume.
        /// </summary>
        Network = 0x0002,
    }
}