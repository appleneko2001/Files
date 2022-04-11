using System;
using System.IO;

namespace Files.Models
{
    public abstract class FileModel
    {
        public FileModel()
        {
        }

        public abstract string GetFullPath();

        public abstract string GetName();

        public abstract ulong GetSize();

        public abstract DateTime GetCreationTime();
        
        public abstract DateTime GetLastAccessTime();
        
        public abstract DateTime GetLastWriteTime();

        public abstract FileAttributes GetAttributes();
    }
}