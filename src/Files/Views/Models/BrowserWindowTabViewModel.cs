using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Threading;
using Files.Commands;
using Files.Views.Models.Breadcrumb;
using Files.Views.Models.Browser;
using Files.Views.Models.Progress;
using Material.Dialog;
using Material.Dialog.Enums;

namespace Files.Views.Models
{
    public class BrowserWindowTabViewModel : HeaderViewModelBase, IDisposable
    {
        private static RelayCommand _selectTabCommand = new RelayCommand(delegate(object o)
        {
            if (o is BrowserWindowTabViewModel vm)
            {
                OnExecuteSelectCommand(vm);
            }
        });
        
        private static ExtendedRelayCommand _closeTabCommand = new ExtendedRelayCommand(delegate(object o)
        {
            if (o is BrowserWindowTabViewModel vm)
            {
                OnExecuteCloseCommand(vm);
            }
        });

        private static RelayCommand _closeSidesheetCommand = new RelayCommand(delegate(object o)
        {
            if (o is BrowserWindowTabViewModel vm)
            {
                vm.IsSidesheetVisible = false;
            }
        });

        private bool _shouldDispose;
        
        private bool _isDisposed;
        private bool _isSelected;
        private bool _isSidesheetVisible;
        private BrowserWindowViewModel _parent;
        private BreadcrumbPathViewModel _breadcrumbPath;
        private BrowserContentViewModelBase _content;
        private BreadcrumbNodeEditViewModel _breadcrumbNodeEdit;
        private CancellationTokenSource _ctx;

        private ProgressViewModel _progress;
        private SidesheetViewModelBase _sidesheet;

        public RelayCommand CloseSidesheetCommand => _closeSidesheetCommand;

        public ExtendedRelayCommand CloseTabCommand => _closeTabCommand;
        
        public RelayCommand SelectTabCommand => _selectTabCommand;

        public BrowserContentViewModelBase Content
        {
            get => _content;
            private set
            {
                _content = value;
                RaiseOnPropertyChangedThroughUiThread();
            }
        }

        public BrowserWindowViewModel Parent => _parent;
        public BreadcrumbPathViewModel BreadcrumbPath => _breadcrumbPath;

        public BreadcrumbNodeEditViewModel BreadcrumbNodeEdit => _breadcrumbNodeEdit;

        public ProgressViewModel Progress
        {
            get => _progress;
            private set
            {
                _progress = value;
                RaiseOnPropertyChanged();
            }
        }
        
        public SidesheetViewModelBase Sidesheet
        {
            get => _sidesheet;
            private set
            {
                _sidesheet = value;
                RaiseOnPropertyChangedThroughUiThread();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            internal set
            {
                _isSelected = value;
                RaiseOnPropertyChanged();
            }
        }

        public bool IsSidesheetVisible
        {
            get => _isSidesheetVisible;
            private set
            {
                _isSidesheetVisible = value;
                RaiseOnPropertyChanged();
            }
        }
        
        public BrowserWindowTabViewModel(BrowserWindowViewModel parent, Uri? path = null)
        {
            _parent = parent;
            _breadcrumbPath = new BreadcrumbPathViewModel(this);
            _breadcrumbNodeEdit = new BreadcrumbNodeEditViewModel(_breadcrumbPath, 0);

            Open(path == null
                ? new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                : path);
        }
        
        private static void OnExecuteCloseCommand(BrowserWindowTabViewModel vm)
        {
            vm.Parent.CloseTab(vm);
        }
        
        private static void OnExecuteSelectCommand(BrowserWindowTabViewModel vm)
        {
            vm.Parent.SelectedTab = vm;
        }
        
        public void Open(Uri path)
        {
            if (_ctx != null)
                _ctx.Cancel();

            _ctx = new CancellationTokenSource();
            var p = new ProgressViewModel();
            Progress = p;
            
            Task.Factory.StartNew(delegate
            {
                p.SetProgress(null);
                
                _breadcrumbPath.ApplyPath(path);
                
                Content = CreateView(path);
                Content.LoadContent(path, _ctx.Token);
                Content.RequestPreviews(_ctx.Token);
            }).ContinueWith(delegate(Task task)
            {
                p.SetProgress(1.0);
                p.SetCompleted();
                
                if(_shouldDispose)
                    Dispose();

                if (!task.IsFaulted && task.IsCompleted)
                    return;

                if (task.IsCanceled)
                    return;

                if (task.Exception?.InnerException is OperationCanceledException)
                    return;

                Dispatcher.UIThread.InvokeAsync(delegate
                {
                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        Borderless = true,
                        ContentHeader = "Error",
                        SupportingText = task.Exception?.Message ?? "Unknown exception.",
                        StartupLocation = WindowStartupLocation.CenterOwner,
                        DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok)
                    });
                    dialog.ShowDialog(Parent.ParentWindow);
                });
            });
        }

        private BrowserContentViewModelBase CreateView(Uri uri)
        {
            Header = HttpUtility.UrlDecode(uri.Segments.Last());
            
            BrowserContentViewModelBase viewModel = null;
            
            switch (uri.Scheme.ToLowerInvariant())
            {
                case "file":
                {
                    viewModel = new LocalFilesBrowserContentViewModel(this);
                } break;
            }

            return viewModel;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            
            _ctx.Dispose();
        }

        internal void AfterClose()
        {
            _ctx.Cancel();

            if (!_progress.IsComplete)
                _shouldDispose = true;
            else
                Dispose();
        }
    }
}