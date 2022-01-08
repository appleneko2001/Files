using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using Avalonia.Threading;
using Material.Icons;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbPathViewModel : ViewModelBase
    {
        private BrowserWindowTabViewModel _parent;
        private ObservableCollection<BreadcrumbNodeViewModel> _part;
        
        
        public BrowserWindowTabViewModel Parent => _parent;
        public ObservableCollection<BreadcrumbNodeViewModel> Part => _part;

        private Uri? _fullPath;

        public BreadcrumbPathViewModel(BrowserWindowTabViewModel parent)
        {
            _parent = parent;
            
            _part = new ObservableCollection<BreadcrumbNodeViewModel>();
        }

        public void ApplyPath(Uri uri)
        {
            _fullPath = uri;
            UpdatePart();
        }

        public void UpdatePart()
        {
            if (_fullPath == null)
                return;
            
            UpdatePartCore(_fullPath);
        }
        
        private void UpdatePartCore(Uri uri)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                Part.Clear();

                int index = 0;
                foreach (var segment in GetSegments(uri))
                {
                    if (index == 0)
                    {
                        var vm = new BreadcrumbNodeSchemeViewModel(this, index, segment);
                        Part.Add(vm);

                        index++;
                        continue;
                    }
                    if (index == 1)
                    {
                        var vm = new BreadcrumbNodeHostViewModel(this, index, uri.Host);
                        Part.Add(vm);
                        
                    }

                    var header = segment;

                    Part.Add(new BreadcrumbNodeViewModel(this, index, segment, header));
                    index++;
                }
            }, DispatcherPriority.Background);
        }

        private IReadOnlyList<string> GetSegments(Uri uri)
        {
            var scheme = $"{uri.Scheme}{Uri.SchemeDelimiter}";
            var path = HttpUtility.UrlDecode(uri.GetLeftPart(UriPartial.Path));

            if (path.StartsWith(scheme))
                path = path.Remove(0, scheme.Length);
            
            var l = new List<string>();
            l.Add(scheme);

            foreach (var segment in path.Split('/'))
            {
                if(!string.IsNullOrEmpty(segment))
                    l.Add($"{segment}/");
            }

            return l;
        }
    }
}