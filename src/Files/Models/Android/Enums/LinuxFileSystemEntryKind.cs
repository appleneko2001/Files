using System;

namespace Files.Models.Android.Enums
{
    [Flags]
    public enum LinuxFileSystemEntryKind
    {
        None = 0,
        File = 1,
        Directory = 2,
        Symlink = 4,
        CharacterDevice = 8,
        BlockDevice = 16,
        Socket = 32,
        Fifo = 64,
    }
}