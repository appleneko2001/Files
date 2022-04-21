using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using Files.ViewModels.Breadcrumb;
using Files.ViewModels.Browser;
using Files.ViewModels.Browser.Files.Local;
using Files.ViewModels.Browser.Properties;
using Files.ViewModels.Browser.Sidesheet;
using Files.ViewModels.Progress;
using Files.ViewModels.Tracker;
using Material.Dialog;
using Material.Dialog.Enums;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels
{
    public partial class BrowserWindowTabViewModel : HeaderViewModelBase, IDisposable
    {
        private bool _shouldDispose;
        private bool _isDisposed;

        private bool _isSelected;
        private bool _isSidesheetVisible;
        private BrowserWindowViewModel _parent;
        private BreadcrumbPathViewModel _breadcrumbPath;
        private BrowserContentViewModelBase? _content;
        private BreadcrumbNodeEditViewModel _breadcrumbNodeEdit;

        private BrowseTrackerViewModel _tracker;
        
        private CancellationTokenSource? _ctx;

        private ProgressViewModel? _progress;
        private SidesheetViewModelBase? _sidesheet;

        public ExtendedRelayCommand CloseTabCommand => _closeTabCommand;

        public ICommand GoBackCommand => _goBackCommand;
        
        public ICommand GoForwardCommand => _goForwardCommand;
        
        public IEnumerable<BrowseTrackerRecordElement> GoBackList => _tracker.GetBackList(); 

        
        public BrowserContentViewModelBase? Content
        {
            get => _content;
            private set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public BrowserWindowViewModel Parent => _parent;
        public BreadcrumbPathViewModel BreadcrumbPath => _breadcrumbPath;

        public BreadcrumbNodeEditViewModel BreadcrumbNodeEdit => _breadcrumbNodeEdit;

        public ProgressViewModel? Progress
        {
            get => _progress;
            private set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public SidesheetViewModelBase? Sidesheet
        {
            get => _sidesheet;
            private set
            {
                _sidesheet = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            internal set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsSidesheetVisible
        {
            get => _isSidesheetVisible;
            set
            {
                _isSidesheetVisible = value;
                OnPropertyChanged();
            }
        }

        public BrowserWindowTabViewModel(BrowserWindowViewModel parent, Uri? path = null)
        {
            _parent = parent;
            _breadcrumbPath = new BreadcrumbPathViewModel(this);
            _breadcrumbNodeEdit = new BreadcrumbNodeEditViewModel(_breadcrumbPath, 0);

            _tracker = new BrowseTrackerViewModel();

            UseHandler();
            
            Open(path == null
                ? new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                : path);
        }

        private static void OnExecuteCloseCommand(BrowserWindowTabViewModel vm)
        {
            vm.Parent.CloseTab(vm);
        }

        public void Open(Uri path, bool record = true)
        {
            _ctx?.Cancel();

            _ctx = new CancellationTokenSource();
            var p = new ProgressViewModel();
            Progress = p;

            Task.Factory.StartNew(delegate
            {
                p.SetProgress(null);

                BreadcrumbPath.ApplyPath(path);
                
                // Unbind events
                Content?.Dispose();

                Content = CreateView(path);

                if (Content == null)
                    throw new NotSupportedException($"The protocol {path.Scheme} is not supported at this moment.");

                OnTrackerStackChanged(this, EventArgs.Empty);
                
                if (record)
                {
                    _tracker.PushAndSetCurrent(new BrowseTrackerRecordElement(path));
                }
                
                Content.LoadContent(path, _ctx.Token);
            }).ContinueWith(delegate(Task task)
            {
                p.SetProgress(1.0);
                p.SetCompleted();

                if (_shouldDispose)
                    Dispose();

                if (!task.IsFaulted && task.IsCompleted)
                    return;

                if (task.IsCanceled)
                    return;

                Exception? exception = task.Exception;

                while (exception is AggregateException e)
                {
                    var innerException = e.InnerException;
                    
                    if (innerException != null)
                        exception = innerException;
                }

                if (exception is OperationCanceledException)
                    return;

                Dispatcher.UIThread.InvokeAsync(delegate
                {
                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        Borderless = true,
                        ContentHeader = "Error",
                        SupportingText = exception?.Message ?? "Unknown exception.",
                        StartupLocation = WindowStartupLocation.CenterOwner,
                        DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok)
                    });
                    dialog.ShowDialog(Parent.ParentWindow);
                });
            });
        }

        public void ShowPropertiesSidesheet(ItemViewModelBase item)
        {
            var vm = new PropertiesSidesheetViewModel();
            Sidesheet = vm;
            IsSidesheetVisible = true;

            switch (item)
            {
                case LocalFileSystemItemViewModel localFs:
                {
                    vm.AppendModel(new CommonFileSystemProperties(localFs));
                }
                    break;
            }
        }

        private BrowserContentViewModelBase? CreateView(Uri uri)
        {
            Header = HttpUtility.UrlDecode(uri.Segments.Last());

            BrowserContentViewModelBase? viewModel = null;

            switch (uri.Scheme.ToLowerInvariant())
            {
                case "file":
                {
                    viewModel = new LocalFilesBrowserContentViewModel(this);
                } break;

                case "adbfile":
                {
                    viewModel = new AdbBrowserContentViewModel(this);
                } break;
            }

            return viewModel;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            
            _isDisposed = true;
            
            GC.SuppressFinalize(this);
            
            _ctx?.Dispose();
        }

        internal void AfterClose()
        {
            _ctx?.Cancel();
            
            RemoveHandler();

            if (!_progress?.IsComplete ?? false)
                _shouldDispose = true;
            else
                Dispose();
        }

        private void UseHandler()
        {
            _tracker.ObservableStack.CollectionChanged += OnTrackerStackChanged;
        }

        private void OnTrackerStackChanged(object sender, EventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                _goBackCommand.RaiseCanExecuteChanged();
                _goForwardCommand.RaiseCanExecuteChanged();

                OnPropertyChanged(nameof(GoBackList));
            });
        }

        private void RemoveHandler()
        {
            _tracker.ObservableStack.CollectionChanged -= OnTrackerStackChanged;
        }
    }
}