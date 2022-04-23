using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Web;
using Avalonia.Threading;
using Files.Extensions;
using MinimalMvvm.ViewModels;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Breadcrumb
{
    public class BreadcrumbPathViewModel : ViewModelBase
    {
        private static RelayCommand _submitEditedPathCommand = new(OnSubmitEditedPath);
        
        private Uri? _fullPath;
        private bool _isInEditMode;
        private string? _editLine;
        
        private BrowserWindowTabViewModel _parent;
        private BreadcrumbNodeEditViewModel _editButton;
        private ObservableCollection<BreadcrumbNodeViewModel> _part;

        public BrowserWindowTabViewModel Parent => _parent;
        public ObservableCollection<BreadcrumbNodeViewModel> Part => _part;
        public Uri? FullPath => _fullPath;
        public RelayCommand SubmitEditedPathCommand => _submitEditedPathCommand;

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                _isInEditMode = value;
                OnPropertyChanged();
                
                _editButton?.UpdateStatus();
                UpdateEditLine();
            }
        }

        public string? EditLine
        {
            get => _editLine;
            set
            {
                _editLine = value;
                OnPropertyChanged();
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

            UpdateEditLine();
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

        private void UpdateEditLine()
        {
            EditLine = _isInEditMode ? HttpUtility.UrlDecode(_fullPath?.AbsoluteUri) : null;
        }

        private static void OnSubmitEditedPath(object? arg)
        {
            if (arg is not BreadcrumbPathViewModel vm)
                return;
            
            var path = vm.EditLine!;
            
            var uri = new Uri(path);
            vm.IsInEditMode = false;
            vm.Parent.Open(uri);
        }

        private void UpdatePartCore(Uri uri)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                Part.Clear();

                var index = 0;
                foreach (var segment in uri.GenerateSegments())
                {
                    switch (index)
                    {
                        case 0:
                        {
                            var vm = new BreadcrumbNodeSchemeViewModel(this, index, segment);
                            Part.Add(vm);

                            index++;
                            continue;
                        }
                        case 1:
                        {
                            var builder = new StringBuilder();

                            var ui = uri.UserInfo;
                            if (!string.IsNullOrWhiteSpace(ui))
                            {
                                builder.Append($"{ui}@");
                            }

                            builder.Append(uri.Host);
                            
                            var vm = new BreadcrumbNodeHostViewModel(this, index, builder.ToString());
                            Part.Add(vm);
                        
                            index++;
                            continue;
                        }
                    }

                    var header = segment;

                    Part.Add(new BreadcrumbNodeViewModel(this, index, segment, header));
                    index++;
                }
            }, DispatcherPriority.Background);
        }
    }
}