using System;
using Files.Commands;
using Files.Models;

namespace Files.Views.Models.Storage
{
    public class StorageEntryViewModel
    {
        private StorageDeviceViewModel _parent;
        // Private fields
        private StorageEntryModel _model;
        
        // Public properties
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
        }

        // Static commands -- Unmount
        private static readonly RelayCommand _unmountCommand = new(ExecuteUnmountCommand, CanExecuteUnmountCommand);
        private static bool CanExecuteUnmountCommand(object arg)
        {
            if (arg is StorageEntryViewModel vm)
            {
                return vm._model.CanUnmount;
            }

            return false;
        }

        private static void ExecuteUnmountCommand(object obj)
        {
            if (obj is StorageEntryViewModel vm)
            {
                vm._model.Unmount();
            }
        }
        // Static command -- Open view to the storage on current tab
        private static RelayCommand _openStorageEntryCommand =
            new RelayCommand(ExecuteOpenStorageEntryOnCurrentTabCommand, CanExecuteOpenStorageEntryOnCurrentTabCommand);

        private static bool CanExecuteOpenStorageEntryOnCurrentTabCommand(object arg)
        {
            if (arg is StorageEntryViewModel entry)
            {
                return entry.IsReady;
            }

            return false;
        }

        private static void ExecuteOpenStorageEntryOnCurrentTabCommand(object arg)
        {
            if (arg is StorageEntryViewModel entry)
            {
                var tab = entry.Parent.Parent.SelectedTab;
                tab.OpenAsync(new Uri(entry.Entry));
            }
        }
    }
}