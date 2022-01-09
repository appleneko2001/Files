using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using Avalonia.Threading;
using Files.Commands;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbPathViewModel : ViewModelBase
    {
        private static RelayCommand _submitEditedPathCommand = new RelayCommand(OnSubmitEditedPath);
        
        private Uri? _fullPath;
        private bool _isInEditMode;
        private string? _editLine;
        
        private BrowserWindowTabViewModel _parent;
        private BreadcrumbNodeEditViewModel _editButton;
        private ObservableCollection<BreadcrumbNodeViewModel> _part;

        public BrowserWindowTabViewModel Parent => _parent;
        public ObservableCollection<BreadcrumbNodeViewModel> Part => _part;
        public RelayCommand SubmitEditedPathCommand => _submitEditedPathCommand;

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                _isInEditMode = value;
                RaiseOnPropertyChanged();

                EditLine = _isInEditMode ? HttpUtility.UrlDecode(_fullPath?.AbsoluteUri) : null;
                _editButton?.UpdateStatus();
            }
        }

        public string? EditLine
        {
            get => _editLine;
            set
            {
                _editLine = value;
                RaiseOnPropertyChanged();
            }
        }

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

        public void ApplyEditButton(BreadcrumbNodeEditViewModel vm)
        {
            _editButton = vm;
        }

        public void UpdatePart()
        {
            if (_fullPath == null)
                return;

            UpdatePartCore(_fullPath);
        }

        private static void OnSubmitEditedPath(object arg)
        {
            if (arg is not BreadcrumbPathViewModel vm)
                return;
            
            var path = vm.EditLine!;
            
            var uri = new Uri(path);
            vm.IsInEditMode = false;
            vm.Parent.OpenAsync(uri);
        }

        private void UpdatePartCore(Uri uri)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                Part.Clear();

                var index = 0;
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
                if (!string.IsNullOrEmpty(segment))
                    l.Add($"{segment}/");

            return l;
        }
    }
}