﻿using System.Diagnostics;
using System.IO;
using Files.Models;

namespace Files.Windows.Models
{
    public class WindowsMediaEntryModel : StorageEntryModel
    {
        private string _procPath;
        private string _entry;
        private string _label;
        private readonly bool _canUnmount;

        public WindowsMediaEntryModel(DriveInfo di)
        {
            _entry = di.RootDirectory.FullName;

            if (di.IsReady)
            {
                _label = di.VolumeLabel;
            }
            
            _canUnmount = di.DriveType switch
            {
                DriveType.Removable => true,
                DriveType.CDRom => true,
                _ => false
            };
        }
        
        public override string ProcPath => _procPath;
        
        public override string Entry => _entry;

        public override string Label => _label;

        public override bool CanUnmount => _canUnmount;
        public override bool IsReady => new DriveInfo(Entry).IsReady;

        protected override void UpdateProcPath(string p)
        {
            _procPath = p;
        }

        protected override void UpdateEntry(string e)
        {
            _entry = e;
        }

        protected override void UpdateLabel(string l)
        {
            _label = l;
        }

        public override void Unmount()
        {
            // TODO: System-level unmount device api calls
            throw new System.NotImplementedException();
        }
    }
}