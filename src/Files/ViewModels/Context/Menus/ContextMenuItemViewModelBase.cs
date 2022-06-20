using Files.ViewModels.Browser;
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
        
        public virtual bool MayExecute(BrowserContentViewModelBase? viewModel)
        {
            return true;
        }
    }
}