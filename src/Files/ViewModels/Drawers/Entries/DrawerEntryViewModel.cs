using System.Windows.Input;
using Files.ViewModels.Drawers.Sections;
using MinimalMvvm.Extensions;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Drawers.Entries
{
    public class DrawerEntryViewModel : ViewModelBase
    {
        public string? Name
        {
            get => _name;
            set => this.SetAndUpdateIfChanged(ref _name, value);
        }

        public IconViewModelBase? Icon
        {
            get => _icon;
            set => this.SetAndUpdateIfChanged(ref _icon, value);
        }

        public ICommand? ClickCommand
        {
            get => _clickCommand;
            set => this.SetAndUpdateIfChanged(ref _clickCommand, value);
        }

        private readonly DrawerSectionViewModel _parent;
        private string? _name;
        private IconViewModelBase? _icon;
        private ICommand? _clickCommand;

        public DrawerEntryViewModel(DrawerSectionViewModel parent)
        {
            _parent = parent;
        }
    }
}