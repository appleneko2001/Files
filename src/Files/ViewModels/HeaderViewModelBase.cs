using Files.ViewModels.Interfaces;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels
{
    public class HeaderViewModelBase : ViewModelBase, IHeaderViewModel
    {
        private object? _icon;
        public virtual object? Icon
        {
            get => _icon;
            protected set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
        
        private string _header;
        public virtual string Header
        {
            get => _header;
            protected set
            {
                _header = value;
                OnPropertyChanged();
            }
        }
    }
}