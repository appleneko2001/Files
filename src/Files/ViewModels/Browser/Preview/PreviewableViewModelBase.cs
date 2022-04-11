using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;

namespace Files.ViewModels.Browser.Preview
{
    public class PreviewableViewModelBase : ViewModelBase
    {
        protected MemoryStream? PrivateMemoryStream;
        protected Bitmap PrivatePreviewInstance;
        protected byte[]? PrivatePreviewData;

        public virtual Bitmap PreviewInstance
        {
            get
            {
                if (PrivatePreviewInstance != null && PrivatePreviewInstance.Size != Size.Empty)
                    return PrivatePreviewInstance;
                
                if (PrivatePreviewData == null || PrivatePreviewData.LongLength <= 0)
                    return null;
                    
                PrivateMemoryStream ??= new MemoryStream(PrivatePreviewData);

                PrivatePreviewInstance = new Bitmap(PrivateMemoryStream);

                return PrivatePreviewInstance;
            }
            protected set
            {
                PrivatePreviewInstance = value;
                RaiseOnPropertyChanged();
            }
        }

        protected PreviewableViewModelBase()
        {
            
        }

        public Stream BeginWritePreviewData()
        {
            if (PrivateMemoryStream != null && PrivateMemoryStream.CanWrite)
                return PrivateMemoryStream;

            PrivateMemoryStream = new MemoryStream();
            return PrivateMemoryStream;
        }

        public void EndWritePreviewData()
        {
            if (PrivateMemoryStream == null)
                return;

            PrivateMemoryStream.Flush();
            PrivatePreviewData = PrivateMemoryStream.GetBuffer();
            
            PrivateMemoryStream.Close();
            PrivateMemoryStream = null;
        }
    }
}