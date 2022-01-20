using System.Threading;

namespace Files.Views.Models.Browser.Preview
{
    public interface IRequestPreviewable
    {
        void TryGetPreview(CancellationToken _cancellationToken = default);
    }
}