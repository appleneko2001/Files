using Material.Icons;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbNodeEditViewModel : BreadcrumbNodeViewModel
    {
        private readonly MaterialIconKind? _iconKind;
        public MaterialIconKind? IconKind => _iconKind;
        
        public BreadcrumbNodeEditViewModel(BreadcrumbPathViewModel parent, int index) : base(parent, index)
        {
            _iconKind = MaterialIconKind.Edit;
            Header = "Edit";
        }

        public override void Click()
        {
            // TODO: Breadcrumb edit line (Editable address line)
        }
    }
}