using Material.Icons;

namespace Files.ViewModels.Breadcrumb
{
    public class BreadcrumbNodeWithIconViewModel : BreadcrumbNodeViewModel
    {
        private MaterialIconKind? _iconKind;
        public MaterialIconKind? IconKind
        {
            get => _iconKind;
            protected set
            {
                _iconKind = value;
                OnPropertyChanged();
            }
        }

        protected BreadcrumbNodeWithIconViewModel(BreadcrumbPathViewModel parent, int index) : base(parent, index)
        {
        }

        public BreadcrumbNodeWithIconViewModel(BreadcrumbPathViewModel parent, int index, string path, string header) : base(parent, index, path, header)
        {
        }
    }
}