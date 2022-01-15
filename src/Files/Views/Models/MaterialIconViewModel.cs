using Material.Icons;

namespace Files.Views.Models
{
    public class MaterialIconViewModel : IconViewModelBase
    {
        public MaterialIconViewModel(MaterialIconKind kind)
        {
            _kind = kind;
        }
        
        private MaterialIconKind _kind;
        public MaterialIconKind Kind => _kind;
    }
}