using System.Threading;

namespace Files.Views.Models.Browser.Home
{
    public class HomeBrowserItemViewModel : ItemViewModelBase
    {
        public HomeBrowserItemViewModel(string name) : base(name)
        {
        }

        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            // TODO: ¯\_(ツ)_/¯
        }
    }
}