using System.Threading;
using System.Windows.Input;
using Avalonia.Controls;
using Files.ViewModels.Browser;
using Material.Icons;

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

        private bool _isReadonly;
        public bool IsReadonly
        {
            get => _isReadonly;
            protected set
            {
                _isReadonly = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private MaterialIconKind? _additionalIconKind;
        public MaterialIconKind? AdditionalIconKind
        {
            get => _additionalIconKind;
            protected set
            {
                _additionalIconKind = value;
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

        public abstract bool IsFolder { get; }
        
        public virtual ICommand? OnClickCommand { get; protected set; }

        public abstract void TryGetPreview(CancellationToken _cancellationToken = default);
        
        public override string ToString()
        {
            return Name;
        }
    }
}