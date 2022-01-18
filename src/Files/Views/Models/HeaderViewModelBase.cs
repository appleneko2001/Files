namespace Files.Views.Models
{
    public class HeaderViewModelBase : ViewModelBase
    {
        private object? _icon;
        public object? Icon
        {
            get => _icon;
            protected set
            {
                _icon = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private string _header;
        public string Header
        {
            get => _header;
            protected set
            {
                _header = value;
                RaiseOnPropertyChanged();
            }
        }
    }
}