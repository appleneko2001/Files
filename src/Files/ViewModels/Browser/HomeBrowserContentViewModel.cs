using System;
using System.Threading;

namespace Files.ViewModels.Browser
{
    public class HomeBrowserContentViewModel : BrowserContentViewModelBase
    {
        protected HomeBrowserContentViewModel(BrowserWindowTabViewModel parent) : base(parent)
        {
        }

        public override void LoadContent(Uri uri, CancellationToken _cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}