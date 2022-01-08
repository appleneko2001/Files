namespace Files.Windows.Services.Native.Devices
{
    /// <summary>
    /// DBT_DEVICE* enums
    /// </summary>
    public enum WinDeviceStatus : uint
    {
        /// <summary>
        /// system detected a new device
        /// </summary>
        DBT_DEVICEARRIVAL              = 0x8000,
        
        /// <summary>
        /// wants to remove, may fail
        /// </summary>
        DBT_DEVICEQUERYREMOVE          = 0x8001,
        
        /// <summary>
        /// removal aborted 
        /// </summary>
        DBT_DEVICEQUERYREMOVEFAILED    = 0x8002,
        
        /// <summary>
        /// about to remove, still avail.
        /// </summary>
        DBT_DEVICEREMOVEPENDING        = 0x8003, 
        
        /// <summary>
        /// device is gone
        /// </summary>
        DBT_DEVICEREMOVECOMPLETE       = 0x8004,
        
        /// <summary>
        /// type specific event
        /// </summary>
        DBT_DEVICETYPESPECIFIC         = 0x8005
    }
}