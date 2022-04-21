using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MinimalMvvm.ViewModels;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Breadcrumb
{
    public class BreadcrumbNodeViewModel : ViewModelBase
    {
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
        
        public ICommand ClickCommand
        {
            get => _clickCommand;
            protected set
            {
                _clickCommand = value;
                OnPropertyChanged();
            }
        }
        
        private BreadcrumbPathViewModel _parent;
        
        private string _header;
        private int _index;
        private string _path;
        
        private ICommand _clickCommand;

        protected BreadcrumbPathViewModel Parent => _parent;

        protected BreadcrumbNodeViewModel(BreadcrumbPathViewModel parent, int index)
        {
            _parent = parent;
            _index = index;

            _clickCommand = new RelayCommand(delegate
            {
                Click();
            });
        }
        
        public BreadcrumbNodeViewModel(BreadcrumbPathViewModel parent, int index, string path, string header)
        {
            _parent = parent;
            _index = index;
            _path = path;
            _header = header;
            
            _clickCommand = new RelayCommand(delegate
            {
                Click();
            });
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