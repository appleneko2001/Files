using System;
using System.IO;

namespace Files.Tasks
{
    public class GetPreviewLocalFileTaskModel : GetPreviewTaskModel
    {
        public FileInfo LocalFileInfo;
        
        public override Stream GetStream()
        {
            if (LocalFileInfo is null)
                throw new ArgumentNullException(nameof(LocalFileInfo));

            return LocalFileInfo.OpenRead();
        }
    }
}