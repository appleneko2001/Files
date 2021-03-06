using System.Threading;
using System.Windows.Input;
using Avalonia.Controls;
using Files.ViewModels.Browser;
using Material.Icons;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels
{
    public abstract class ItemViewModelBase : ViewModelBase, ISelectable
    {
        private BrowserContentViewModelBase _parent;
        public BrowserContentViewModelBase Parent => _parent;
        
        protected ItemViewModelBase(BrowserContentViewModelBase parent)
        {
            _parent = parent;
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
                OnPropertyChanged();
            }
        }
        
        private string? _displayName;
        public string DisplayName
        {
            get => _displayName ?? Name;
            protected set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            protected set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _isReadonly;
        public bool IsReadonly
        {
            get => _isReadonly;
            protected set
            {
                _isReadonly = value;
                OnPropertyChanged();
            }
        }
        
        private MaterialIconKind? _additionalIconKind;
        public MaterialIconKind? AdditionalIconKind
        {
            get => _additionalIconKind;
            protected set
            {
                _additionalIconKind = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public abstract bool IsFolder { get; }
        
        public virtual ICommand? OnClickCommand { get; protected set; }

        public abstract void TryGetPreview(CancellationToken _cancellationToken = default);
        
        public override string ToString()
        {
            return Name;
        }
    }
}