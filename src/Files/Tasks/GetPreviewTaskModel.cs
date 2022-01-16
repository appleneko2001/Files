using System.IO;
using Avalonia.Visuals.Media.Imaging;
using Files.Views.Models.Browser.Preview;

namespace Files.Tasks
{
    public abstract class GetPreviewTaskModel : TaskResultModel<PreviewableViewModelBase>
    {
        public int TargetPreviewSize = 120;

        public BitmapInterpolationMode BitmapQuality = BitmapInterpolationMode.LowQuality;
        
        public abstract Stream GetStream();
    }
}