using System;
using System.IO;

namespace Files.Models
{
    public class LocalFileModel : FileModel
    {
        private string _path;
        private string _name;
        private ulong _size;
        private DateTime _creationTime;
        private DateTime _lastAccessTime;
        private DateTime _lastWriteTime;
        private FileAttributes _attr;
        
        public LocalFileModel(string path)
        {
            _path = path;

            var fi = new FileInfo(_path);
            _name = fi.Name;
            _size = (ulong)fi.Length;
            _creationTime = fi.CreationTimeUtc;
            _lastAccessTime = fi.LastAccessTimeUtc;
            _lastWriteTime = fi.LastWriteTimeUtc;
            _attr = fi.Attributes;
        }

        public override string GetFullPath() => _path;

        public override string GetName() => _name;

        public override ulong GetSize() => _size;

        /// <summary>
        /// Get the creation time of file. Should coordinated in UTC.
        /// </summary>
        public override DateTime GetCreationTime() => _creationTime;

        public override DateTime GetLastAccessTime() => _lastAccessTime;

        public override DateTime GetLastWriteTime() => _lastWriteTime;

        public override FileAttributes GetAttributes() => _attr;
    }
}