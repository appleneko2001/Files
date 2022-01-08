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
        
        public BrowserWindowTabViewModel(BrowserWindowViewModel parent, Uri? path = null)
        {
            _parent = parent;
            _breadcrumbPath = new BreadcrumbPathViewModel(this);
            _breadcrumbNodeEdit = new BreadcrumbNodeEditViewModel(_breadcrumbPath, 0);

            Open(path == null
                ? new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                : path);
        }

        public void OpenAsync(Uri path)
        {
            Task.Run(delegate
            {
                Open(path);
            });
        }
        
        public void Open(Uri path)
        {
            try
            {
                Content = CreateView(path);
                
                _breadcrumbPath.ApplyPath(path);
            }
            catch (Exception e)
            {
                Dispatcher.UIThread.InvokeAsync(delegate
                {
                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        Borderless = true,
                        ContentHeader = "Error",
                        SupportingText = e.Message,
                        StartupLocation = WindowStartupLocation.CenterOwner,
                        DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok)
                    });
                    dialog.ShowDialog(Parent.ParentWindow);
                });
            }
        }

        private BrowserContentViewModelBase CreateView(Uri uri)
        {
            var path = HttpUtility.UrlDecode(uri.AbsolutePath);
            BrowserContentViewModelBase viewModel = null;
            
            switch (uri.Scheme.ToLowerInvariant())
            {
                case "file":
                {
                    viewModel = new LocalFilesBrowserContentViewModel(this, path);
                } break;
            }

            return viewModel;
        }
    }
}