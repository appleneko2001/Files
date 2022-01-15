using System.Windows.Input;

namespace Files.Views.Models.Context.Menus
{
    public class ContextMenuItemViewModel : ContextMenuItemViewModelBase
    {
        public ContextMenuItemViewModel(string header, IconViewModelBase? icon = null, ICommand command = null)
        {
            _command = command;
            _header = header;
            _icon = icon;
        }
        
        private IconViewModelBase? _icon;
        public IconViewModelBase? Icon => _icon;

        private string _header;
        public string Header => _header;

        private ICommand? _command;
        public ICommand? Command => _command;
    }
}