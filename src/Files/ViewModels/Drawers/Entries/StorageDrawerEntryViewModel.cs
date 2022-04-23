using System;
using System.Windows.Input;
using Files.Models;
using Files.Models.Devices.Enums;
using Files.ViewModels.Drawers.Sections;
using Material.Icons;
using MinimalMvvm.Extensions;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Drawers.Entries
{
    public class StorageDrawerEntryViewModel : DrawerEntryViewModel
    {
        public string? Label
        {
            get => _label;
            protected set => this.SetAndUpdateIfChanged(ref _label, value);
        }
        
        public ICommand? ExtraCommand
        {
            get => _extraCommand;
            protected set => this.SetAndUpdateIfChanged(ref _extraCommand, value);
        }

        public StorageEntryModel GetModel() => _model;
        
        public bool IsRemovable => _isRemovable;
        
        private readonly StorageEntryModel _model;
        private readonly Uri _uri;
        private string? _label;
        private bool _isRemovable;
        private ICommand? _extraCommand;

        public StorageDrawerEntryViewModel(DrawerSectionViewModel parent, StorageEntryModel entry)
            :base(parent)
        {
            _model = entry;
            Label = entry.Label;
            Name = entry.Entry;
            _uri = new Uri(entry.Entry);

            MaterialIconKind? iconKind;
            switch (entry.EntryKind)
            {
                case DeviceKind.StaticStorage:
                    iconKind = MaterialIconKind.Harddisk;
                    break;
                case DeviceKind.UsbStorage:
                    iconKind = MaterialIconKind.UsbFlashDrive;
                    break;
                case DeviceKind.CDROM:
                case DeviceKind.CDRW:
                case DeviceKind.DVDROM:
                case DeviceKind.DVDRW:
                    iconKind = MaterialIconKind.Album;
                    break;
                case DeviceKind.Unknown:
                    iconKind = MaterialIconKind.HelpCircleOutline;
                    break;
                case DeviceKind.Sdcard:
                    iconKind = MaterialIconKind.SdCard;
                    break;
                default:
                    iconKind = null;
                    break;
            }

            if(iconKind.HasValue)
                Icon = new MaterialIconViewModel(iconKind.Value);

            ClickCommand = new RelayCommand(delegate
            {
                var tab = parent.Parent.SelectedTab;

                if (tab == null)
                    return;

                tab.Open(_uri);
            });

            if (entry.CanUnmount)
            {
                _isRemovable = true;
                ExtraCommand = new RelayCommand(delegate
                {
                    entry.Unmount();
                });
            }
        }
    }
}