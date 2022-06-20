using System.Windows.Input;
using Avalonia.Input;

namespace Files.ViewModels.Context.Menus
{
    public class StandardContextMenuItemViewModel :
        ContextMenuItemViewModelBase
    {
        public virtual ICommand Command { get; protected set; }
        public virtual IconViewModelBase? Icon { get; protected set; }
        public virtual string? Header { get; protected set; }
        public virtual KeyGesture? Shortcut { get; protected set; }
    }
}