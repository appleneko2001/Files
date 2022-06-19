using Material.Icons;

namespace Files.ViewModels.Breadcrumb
{
    public class BreadcrumbNodeEditViewModel : BreadcrumbNodeWithIconViewModel
    {
        public BreadcrumbNodeEditViewModel(BreadcrumbPathViewModel parent, int index) : base(parent, index)
        {
            parent.ApplyEditButton(this);
            OnStatusChanged();
        }

        public override void Click()
        {
            Parent.IsInEditMode = !Parent.IsInEditMode;

            OnStatusChanged();
        }

        public void UpdateStatus() => OnStatusChanged();

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