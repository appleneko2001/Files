using System;
using System.Linq;
using System.Text;

namespace Files.Views.Models.Breadcrumb
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
                RaiseOnPropertyChangedThroughUiThread();
            }
        }
        
        public int Index 
        {
            get => _index;
            protected set
            {
                _index = value;
                RaiseOnPropertyChangedThroughUiThread();
            }
        }
        
        public string Path
        {
            get => _path;
            protected set
            {
                _path = value;
                RaiseOnPropertyChangedThroughUiThread();
            }
        }

        public virtual void Click()
        {
            var clone = _parent.Part.ToList();

            var builder = new StringBuilder();
            
            foreach (var item in clone)
            {
                builder.Append(item.Path);
                
                if (item == this)
                    break;
            }
            
            _parent.Parent.Open(new Uri(builder.ToString()));
        }
    }
}