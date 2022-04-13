using System.Windows.Input;
using MinimalMvvm.ViewModels;

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
                OnPropertyChanged();
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