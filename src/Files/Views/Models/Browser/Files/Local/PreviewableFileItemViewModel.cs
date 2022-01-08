using System.IO;
using Files.Views.Models.Browser.Preview;

namespace Files.Views.Models.Browser.Files.Local
{
    public class PreviewableFileItemViewModel : FileItemViewModel
    {
        private PreviewableViewModelBase _previewViewModel;
        public PreviewableViewModelBase Preview => _previewViewModel;
        
        // the word Preview + able is present on wiktionary so Rider plz stop to warn me :)
        // https://en.wiktionary.org/wiki/previewable
        public PreviewableFileItemViewModel(LocalFilesBrowserContentViewModel parent, FileInfo fi) : base(parent, fi)
        {
            
        }
    }
}