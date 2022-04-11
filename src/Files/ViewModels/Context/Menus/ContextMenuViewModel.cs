using System.Windows.Input;
using Avalonia.Input;

namespace Files.ViewModels.Context.Menus
{
    public class ContextMenuItemViewModel : ContextMenuItemViewModelBase
    {
        private static readonly ContextMenuItemViewModel _separator = new("-");
        public static ContextMenuItemViewModel Separator => _separator;
        
        public ContextMenuItemViewModel(string header, IconViewModelBase? icon = null, ICommand command = null, KeyGesture? keyGesture = null) : base(command)
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