using System.Windows.Input;
using Avalonia.Input;

namespace Files.ViewModels.Context.Menus
{
    public class ContextMenuItemViewModel : ContextMenuItemViewModelBase
    {
        public static readonly ContextMenuItemViewModel Separator = new("-");
        
        public ContextMenuItemViewModel(string header,
            IconViewModelBase? icon = null,
            ICommand? command = null,
            KeyGesture? keyGesture = null)
        {
            _header = header;
            _keyGesture = keyGesture;
            _icon = icon;
        }
        
        private IconViewModelBase? _icon;
        public IconViewModelBase? Icon => _icon;

        private string _header;
        public string Header => _header;

        private KeyGesture? _keyGesture;
        public KeyGesture? KeyGesture => _keyGesture;
    }
}