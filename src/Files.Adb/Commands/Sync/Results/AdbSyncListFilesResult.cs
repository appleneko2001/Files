using Files.Adb.Models.Sync;

namespace Files.Adb.Commands.Sync
{
    public class AdbSyncListFilesResult : AdbSyncCommandResultBase
    {
        public AdbFileEntry? Result { get; set; }
    }
}