using System.Collections.ObjectModel;

namespace Files.Views.Models.Browser
{
    public class BrowserContentViewModelBase : ViewModelBase
    {
        private BrowserWindowTabViewModel _parent;
        public BrowserWindowTabViewModel Parent => _parent;
        
        private ObservableCollection<ItemViewModelBase> _content;
        public ObservableCollection<ItemViewModelBase> Content => _content;

        protected BrowserContentViewModelBase(BrowserWindowTabViewModel parent)
        {
            _parent = parent;
            _content = new ObservableCollection<ItemViewModelBase>();
        }
    }
}