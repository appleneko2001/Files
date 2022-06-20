using System.IO;
using System.Threading.Tasks;
using Files.Services.Platform.Interfaces;

namespace Files.Windows.Services.Platform
{
    public class OpenWithApplicationHandler : IPlatformSupportShowOpenWithDialog
    {
        public bool CanHandleThisFile(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".exe" => false,
                ".com" => false,
                ".lnk" => false,
                ".url" => false,
                _ => true
            };
        }

        /// <summary>
        /// Show "open file with" dialog. Feature is supported on Windows OS only, not sure how to implement in Unix.
        /// </summary>
        /// <param name="path">The file location.</param>
        public void ShowOpenWithApplicationDialog(string path)
        {
            Task.Factory.StartNew(WindowsApiBridge.OpenFileWithProgramProcedure, path)
                .ContinueWith(WindowsApiBridge.PostOpenFileProcedureTask, path);
        }
    }
}