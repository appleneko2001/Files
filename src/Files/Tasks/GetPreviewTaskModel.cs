using System.IO;
using Files.Views.Models.Browser.Preview;

namespace Files.Tasks
{
    public abstract class GetPreviewTaskModel : TaskResultModel<PreviewableViewModelBase>
    {
        public int TargetPreviewSize = 160;
        
        public abstract Stream GetStream();
    }
}