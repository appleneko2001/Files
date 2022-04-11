namespace Files.Adb.Apis
{
    public class AdbRequestStrings
    {
        /// <summary>
        /// <p>Returns the version of the ADB host.</p>
        /// </summary>
        public static string Version => "host:version";
        
        /// <summary>
        /// <p>Returns the list of all connected devices</p>
        /// </summary>
        public static string GetDevicesList => "host:devices";
        
        /// <summary>
        /// <p>Returns the list of all connected devices and a simple information of each devices.</p>
        /// </summary>
        public static string GetDevicesFullList => "host:devices-l";
        
        /// <summary>
        /// <p>Set device in current session of adb.</p>
        /// <p>Parameters:</p>
        /// <list type="bullet">
        /// id - device id</list>
        /// </summary>
        public static string SetDevice => "host:transport:{id}";
        
        /// <summary>
        /// <p>Send command to device. Requires pass a argument named 'command'.</p>
        /// <p>Parameters:</p>
        /// <list type="bullet">
        /// <item>args - shell arguments</item>
        /// <item>command - command to send to device</item>
        /// </list>
        /// </summary>
        public static string Shell => "shell{args}:{command}";
        
        /// <summary>
        /// <p>Send command to device. Requires pass a argument named 'command'.</p>
        /// <p>Parameters:</p>
        /// <list type="bullet">
        /// command - command to send to device</list>
        /// </summary>
        public static string Exec => "exec:{command}";
    }
}