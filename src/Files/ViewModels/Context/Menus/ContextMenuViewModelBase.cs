using System.Windows.Input;

namespace Files.ViewModels.Context.Menus
{
    public class ContextMenuItemViewModelBase : ViewModelBase
    {
        private object? _commandParameter;

        public object? CommandParameter
        {
            get => _commandParameter;
            set
            {
                _commandParameter = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private ICommand? _command;
        public ICommand? Command => _command;
        
        public ContextMenuItemViewModelBase(ICommand? command)
        {
            _command = command;
        }
    }
}