using System;
using System.Linq;
using System.Text;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Breadcrumb
{
    public class BreadcrumbNodeViewModel : ViewModelBase
    {
        private BreadcrumbPathViewModel _parent;
        
        private string _header;
        private int _index;
        private string _path;

        protected BreadcrumbPathViewModel Parent => _parent;
        
        protected BreadcrumbNodeViewModel(BreadcrumbPathViewModel parent, int index)
        {
            _parent = parent;
            _index = index;
        }
        
        public BreadcrumbNodeViewModel(BreadcrumbPathViewModel parent, int index, string path, string header)
        {
            _parent = parent;
            _index = index;
            _path = path;
            _header = header;
        }

        public string Header
        {
            get => _header;
            protected set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public int Index 
        {
            get => _index;
            protected set
            {
                _index = value;
                OnPropertyChanged();
            }
        }
        
        public string Path
        {
            get => _path;
            protected set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public virtual void Click()
        {
            var clone = _parent.Part.ToList();

            var builder = new StringBuilder();
            
            foreach (var item in clone)
            {
                var part = item.Path;
                
                builder.Append(part);
                
                if (!part.EndsWith("/"))
                    builder.Append("/");
                
                if (item == this)
                    break;
            }
            
            _parent.Parent.Open(new Uri(builder.ToString()));
        }
    }
}