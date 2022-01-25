using System;
using Files.Views.Models.Browser.Properties;

namespace Files.Views.Models.Browser.Sidesheet
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
                RaiseOnPropertyChanged();
            }
        }
        
        public void Dispose()
        {
        }
    }
}