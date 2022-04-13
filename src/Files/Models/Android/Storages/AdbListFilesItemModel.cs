using System;
using System.IO;
using System.Text;
using Files.Adb.Enums;
using Files.Adb.Models.Sync;
using Files.Extensions;
using Files.Models.Android.Enums;

namespace Files.Models.Android.Storages
{
    public class AdbListFilesItemModel
    {
        private string _lsLine;
        private bool _isSecured;
        
        public string? Owner { get; set; }
        public string? Group { get; set; }
        
        public LinuxFileSystemPermissionKind OwnerPermissions { get; set; }
        public LinuxFileSystemPermissionKind GroupPermissions { get; set; }
        public LinuxFileSystemPermissionKind OtherPermissions { get; set; }
        
        public DateTime? LastModified { get; set; }
        public long? Size { get; set; } = -1;
        
        public string Name { get; set; }
        public string FullPath { get; set; }

        public string? LinkTarget { get; set; }
        
        public LinuxFileSystemEntryKind Kind { get; set; }
        
        public bool IsDirectory => Kind == LinuxFileSystemEntryKind.Directory;
        public bool IsSymlink => Kind == LinuxFileSystemEntryKind.Symlink;
        public bool IsHidden => Name?.StartsWith(".") ?? false;
        public bool IsSecured => _isSecured;

        public void Parse(string line, string parent)
        {
            _isSecured = false;
            _lsLine = line;
            // d?????????   ? ?      ?             ?                ? data_mirror
            // drwx--x--x   5 shell  everybody   100 2022-03-27 19:05 storage/
            using (var reader = new StringReader(line))
            {
                var mode = 0;

                var startBlock = false;

                var builder = new StringBuilder();

                void PostAppends()
                {
                    switch (mode)
                    {
                        case 0:
                        {
                            ParseKindAndPermissionsCore(builder.ToString());
                        } break;

                        case 1:
                        {
                            // Second mode means how much entries are in the directory
                            // We don't need it at the moment
                        } break;
                        
                        case 2:
                        {
                            // Third mode means the owner of the file
                            var owner = builder.ToString();

                            Owner = owner == "?" ? null : owner;
                        } break;
                        
                        case 3:
                        {
                            // Fourth mode means the group of the file
                            var group = builder.ToString();
                            
                            Group = group == "?" ? null : group;
                        } break;
                        
                        case 4:
                        {
                            // Fifth mode means the size of the file
                            if (long.TryParse(builder.ToString(), out var size))
                                Size = size;
                            else
                                Size = null;
                        } break;
                        
                        case 5:
                        {
                            // Sixth mode means the last modified date of the file
                            if(DateTime.TryParse(builder.ToString(), out var date))
                                LastModified = date;
                            else
                                LastModified = null;
                        } break;
                        
                        case 6:
                        {
                            // Seventh mode means the name of the file
                            var name = builder.ToString();

                            // If the entry is a link, we need to parse the target
                            if (Kind == LinuxFileSystemEntryKind.Symlink)
                            {
                                var pair = name.Split(new[] { " -> " },
                                    StringSplitOptions.RemoveEmptyEntries);
                                if (pair.Length == 2)
                                {
                                    Name = pair[0];
                                    LinkTarget = pair[1];
                                    
                                    FullPath = parent + pair[0];
                                }
                                else
                                {
                                    Name = name;
                                    FullPath = parent + name;
                                }
                            }
                            else
                            {
                                Name = name;
                                FullPath = parent + name;
                            }
                        } break;
                    }

                    mode++;
                    builder.Clear();
                }

                var timeAppend = false;

                while (reader.GetNextChar(out var c))
                {
                    if (!startBlock)
                    {
                        if(c == ' ')
                            continue;
                    }
                    else
                    {
                        if (c == ' ')
                        {
                            switch (mode)
                            {
                                case < 5:
                                    startBlock = false;
                                    PostAppends();
                                    continue;
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                case 5 when timeAppend || builder.ToString() == "?":
                                    startBlock = false;
                                    PostAppends();
                                    continue;
                                case 5:
                                    timeAppend = true;
                                    break;
                            }
                        }
                    }

                    startBlock = true;
                    builder.Append(c);
                }

                
                if(builder.Length > 0)
                    PostAppends();
            }
        }

        // Thanks copilot )
        private void ParseKindAndPermissionsCore(string line)
        {
            LinuxFileSystemPermissionKind ParsePermissions(char c)
            {
                switch (c)
                {
                    case 'r':
                        return LinuxFileSystemPermissionKind.Read;
                    case 'w':
                        return LinuxFileSystemPermissionKind.Write;
                    case 'x':
                        return LinuxFileSystemPermissionKind.Execute;
                    case '-':
                        return LinuxFileSystemPermissionKind.None;
                    
                    // Might unable to access the entry and show the permissions as '?'
                    case '?':
                        _isSecured = true;
                        return LinuxFileSystemPermissionKind.None;
                    
                    default:
                        throw new Exception("Unknown char");
                }
            }

            for (var i = 0; i < 10; i++)
            {
                var c = line[i];

                switch (i)
                {
                    case 0:
                    {
                        Kind = c switch
                        {
                            'd' => LinuxFileSystemEntryKind.Directory,
                            '-' => LinuxFileSystemEntryKind.File,
                            'l' => LinuxFileSystemEntryKind.Symlink,
                            'c' => LinuxFileSystemEntryKind.CharacterDevice,
                            'b' => LinuxFileSystemEntryKind.BlockDevice,
                            's' => LinuxFileSystemEntryKind.Socket,
                            'p' => LinuxFileSystemEntryKind.Fifo,
                            _ => throw new Exception("Unknown entry kind")
                        };
                        continue;
                    }

                    case 1:
                    case 2:
                    case 3:
                    {
                        OwnerPermissions |= ParsePermissions(c);
                        continue;
                    }

                    case 4:
                    case 5:
                    case 6:
                    {
                        GroupPermissions |= ParsePermissions(c);
                        continue;
                    }
                    
                    case 7:
                    case 8:
                    case 9:
                    {
                        OtherPermissions |= ParsePermissions(c);
                        continue;
                    }
                }
            }
        }

        public void Apply(AdbFileEntry entry)
        {
            Name = Path.GetFileName(entry.Path);
            FullPath = entry.Path;

            if(entry.Mode.HasFlag(UnixFileMode.Regular))
                Kind |= LinuxFileSystemEntryKind.File;
            if(entry.Mode.HasFlag(UnixFileMode.Directory))
                Kind |= LinuxFileSystemEntryKind.Directory;
            if(entry.Mode.HasFlag(UnixFileMode.SymbolicLink))
                Kind |= LinuxFileSystemEntryKind.Symlink;
            if(entry.Mode.HasFlag(UnixFileMode.Character))
                Kind |= LinuxFileSystemEntryKind.CharacterDevice;
            if(entry.Mode.HasFlag(UnixFileMode.Block))
                Kind |= LinuxFileSystemEntryKind.BlockDevice;

            LastModified = entry.DateTime;
            Size = entry.Size;
        }
    }
}