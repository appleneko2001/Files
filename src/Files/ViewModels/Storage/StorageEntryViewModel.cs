using System;
using Files.Models;
using Files.Models.Devices.Enums;
using Material.Icons;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Storage
{
    public class StorageEntryViewModel
    {
        // Private fields
        private StorageDeviceViewModel _parent;
        private StorageEntryModel _model;
        private MaterialIconKind? _iconKind;
        
        // Public properties
        public MaterialIconKind? IconKind => _iconKind;
        public StorageDeviceViewModel Parent => _parent;
        public string Label => _model.Label;
        public string Entry => _model.Entry;
        public bool IsReady => _model.IsReady;

        public bool IsRemovable => _model.IsReady && _model.CanUnmount;
        
        // View model commands
        public RelayCommand UnmountCommand => _unmountCommand;

        public RelayCommand OpenStorageEntryCommand => _openStorageEntryCommand;

        public StorageEntryViewModel(StorageDeviceViewModel parent, StorageEntryModel entry)
        {
            _parent = parent;
            _model = entry;

            _iconKind = entry.EntryKind switch
            {
                DeviceKind.StaticStorage => MaterialIconKind.Harddisk,
                DeviceKind.UsbStorage => MaterialIconKind.UsbFlashDrive,
                DeviceKind.CDROM => MaterialIconKind.Album,
                DeviceKind.Unknown => MaterialIconKind.HelpCircleOutline,
                _ => null
            };
        }

        // Static commands -- Unmount
        private static readonly RelayCommand _unmountCommand = new(ExecuteUnmountCommand, CanExecuteUnmountCommand);
        private static bool CanExecuteUnmountCommand(object? arg)
        {
            if (arg is StorageEntryViewModel vm)
            {
                return vm._model.CanUnmount;
            }

            return false;
        }

        private static void ExecuteUnmountCommand(object? obj)
        {
            if (obj is StorageEntryViewModel vm)
            {
                vm._model.Unmount();
            }
        }
        // Static command -- Open view to the storage on current tab
        private static RelayCommand _openStorageEntryCommand =
            new(ExecuteOpenStorageEntryOnCurrentTabCommand, CanExecuteOpenStorageEntryOnCurrentTabCommand);

        private static bool CanExecuteOpenStorageEntryOnCurrentTabCommand(object? arg)
        {
            if (arg is StorageEntryViewModel entry)
            {
                return entry.IsReady;
            }

            return false;
        }

        private static void ExecuteOpenStorageEntryOnCurrentTabCommand(object? arg)
        {
            if (arg is not StorageEntryViewModel entry)
                return;
            
            var tab = entry.Parent.Parent.SelectedTab;
            tab.Open(new Uri(entry.Entry));
        }
    }
}