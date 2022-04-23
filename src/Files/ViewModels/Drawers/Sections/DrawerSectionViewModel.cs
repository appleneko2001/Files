using System.Collections.ObjectModel;
using Files.ViewModels.Drawers.Entries;
using MinimalMvvm.Extensions;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Drawers.Sections
{
    public class DrawerSectionViewModel : ViewModelBase
    {
        public string? Header
        {
            get => _header;
            protected set => this.SetAndUpdateIfChanged(ref _header, value);
        }

        public IconViewModelBase? Icon
        {
            get => _icon;
            protected set => this.SetAndUpdateIfChanged(ref _icon, value);
        }

        public BrowserWindowViewModel Parent => _parent;
        
        public ObservableCollection<DrawerEntryViewModel> Entries => _entries;
        
        private readonly ObservableCollection<DrawerEntryViewModel> _entries;
        private readonly BrowserWindowViewModel _parent;
        private string? _header;
        private IconViewModelBase? _icon;

        public DrawerSectionViewModel(BrowserWindowViewModel parent)
        {
            _parent = parent;
            _entries = new ObservableCollection<DrawerEntryViewModel>();
        }
    }
}