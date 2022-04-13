using System;
using Files.ViewModels.Browser.Properties;

namespace Files.ViewModels.Browser.Sidesheet
{
    public class PropertiesSidesheetViewModel : SidesheetViewModelBase, IDisposable
    {
        public void AppendModel(FileSystemPropertiesViewModelBase props)
        {
            Properties = props;
        }

        private FileSystemPropertiesViewModelBase _properties;
        public FileSystemPropertiesViewModelBase Properties
        {
            get => _properties;
            set
            {
                _properties = value;
                OnPropertyChanged();
            }
        }
        
        public void Dispose()
        {
        }
    }
}