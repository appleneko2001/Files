using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace Files.Views.Models.Browser
{
    public abstract class BrowserContentViewModelBase : ViewModelBase
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

        public abstract void LoadContent(Uri uri, CancellationToken _cancellationToken = default);

        public abstract void RequestPreviews(CancellationToken _cancellationToken = default);
    }
}