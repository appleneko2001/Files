using Material.Icons;

namespace Files.ViewModels
{
    public class MaterialIconViewModel : IconViewModelBase
    {
        public static MaterialIconViewModel Launch { get; } = new (MaterialIconKind.Launch);
        
        public MaterialIconViewModel(MaterialIconKind kind)
        {
            _kind = kind;
        }
        
        private MaterialIconKind _kind;
        public MaterialIconKind Kind => _kind;
    }
}