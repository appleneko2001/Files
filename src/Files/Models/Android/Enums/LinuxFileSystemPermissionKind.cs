using System;

namespace Files.Models.Android.Enums
{
    [Flags]
    public enum LinuxFileSystemPermissionKind : byte
    {
        // Unknown or no permissions
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }
}