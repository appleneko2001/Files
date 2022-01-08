using System;
using System.IO;

namespace Files.Models
{
    public class LocalFolderModel : FileModel
    {
        private string _path;
        private string _name;
        private DateTime _creationTime;
        private DateTime _lastAccessTime;
        private DateTime _lastWriteTime;
        private FileAttributes _attr;

        public LocalFolderModel(string path)
        {
            _path = path;
            var di = new DirectoryInfo(path);

            _name = di.Name;
            _creationTime = di.CreationTimeUtc;
            _lastAccessTime = di.LastAccessTimeUtc;
            _lastWriteTime = di.LastWriteTimeUtc;
            _attr = di.Attributes;
        }
        
        public override string GetFullPath() => _path;

        public override string GetName() => _name;

        public override ulong GetSize()
        {
            // TODO: Get folder size
            
            
            return 0;
        }

        /// <summary>
        /// Get the creation time of file. Should coordinated in UTC.
        /// </summary>
        public override DateTime GetCreationTime() => _creationTime;

        public override DateTime GetLastAccessTime() => _lastAccessTime;

        public override DateTime GetLastWriteTime() => _lastWriteTime;

        public override FileAttributes GetAttributes() => _attr;
    }
}