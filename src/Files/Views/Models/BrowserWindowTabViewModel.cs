using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Threading;
using Files.Views.Models.Breadcrumb;
using Files.Views.Models.Browser;
using Material.Dialog;
using Material.Dialog.Enums;

namespace Files.Views.Models
{
    public class BrowserWindowTabViewModel : ViewModelBase
    {
        private BrowserWindowViewModel _parent;
        private BreadcrumbPathViewModel _breadcrumbPath;
        private BrowserContentViewModelBase _content;
        private BreadcrumbNodeEditViewModel _breadcrumbNodeEdit;
        private CancellationTokenSource _ctx;

        private ProgressViewModel _progress;

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
        
        public BrowserWindowTabViewModel(BrowserWindowViewModel parent, Uri? path = null)
        {
            _parent = parent;
            _breadcrumbPath = new BreadcrumbPathViewModel(this);
            _breadcrumbNodeEdit = new BreadcrumbNodeEditViewModel(_breadcrumbPath, 0);

            Open(path == null
                ? new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                : path);
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
                Content.LoadContent(path);
                Content.RequestPreviews(_ctx.Token);
            }).ContinueWith(delegate(Task task)
            {
                p.SetProgress(1.0);
                p.SetCompleted();
                
                if (!task.IsFaulted && task.IsCompleted)
                    return;

                if (task.IsCanceled)
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
            var path = HttpUtility.UrlDecode(uri.AbsolutePath);
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
    }
}