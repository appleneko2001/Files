using Material.Icons;

namespace Files.Views.Models.Breadcrumb
{
    public class BreadcrumbNodeEditViewModel : BreadcrumbNodeWithIconViewModel
    {
        public BreadcrumbNodeEditViewModel(BreadcrumbPathViewModel parent, int index) : base(parent, index)
        {
            OnStatusChanged();
        }

        public override void Click()
        {
            // TODO: Breadcrumb edit line (Editable address line)
            Parent.IsInEditMode = !Parent.IsInEditMode;

            OnStatusChanged();
        }

        private void OnStatusChanged()
        {
            IconKind = Parent.IsInEditMode switch
            {
                true => MaterialIconKind.Close,
                false => MaterialIconKind.Edit
            };
            
            Header = Parent.IsInEditMode switch
            {
                true => "Cancel",
                false => "Edit"
            };
        }
    }
}