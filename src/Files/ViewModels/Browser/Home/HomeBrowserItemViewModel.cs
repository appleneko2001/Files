using System.Threading;

namespace Files.ViewModels.Browser.Home
{
    public class HomeBrowserItemViewModel : ItemViewModelBase
    {
        public HomeBrowserItemViewModel(string name) : base(name)
        {
        }

        public override bool IsFolder => false;

        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            // TODO: ¯\_(ツ)_/¯
        }
    }
}