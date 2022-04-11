using System.Threading;

namespace Files.ViewModels.Browser.Preview
{
    public interface IRequestPreviewable
    {
        void TryGetPreview(CancellationToken _cancellationToken = default);
    }
}