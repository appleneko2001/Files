using System.Threading;
using Avalonia.Controls;
using Files.Commands;

namespace Files.Views.Models
{
    public abstract class ItemViewModelBase : ViewModelBase, ISelectable
    {
        protected ItemViewModelBase()
        {
        }
        
        public ItemViewModelBase(string name)
        {
            Name = name;
            DisplayName = name;
        }
        
        private string _name;
        public string Name
        {
            get => _name;
            protected set
            {
                _name = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            protected set
            {
                _displayName = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            protected set
            {
                _isVisible = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaiseOnPropertyChanged();
            }
        }
        
        public virtual RelayCommand? OnClickCommand { get; }

        public abstract void TryGetPreview(CancellationToken _cancellationToken = default);
    }
}