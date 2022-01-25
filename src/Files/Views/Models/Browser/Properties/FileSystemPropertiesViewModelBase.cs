using System.Threading;
using System.Threading.Tasks;
using Files.Views.Models.Interfaces;

namespace Files.Views.Models.Browser.Properties
{
    public class FileSystemPropertiesViewModelBase : ViewModelBase, ITaskCancellable
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            protected set
            {
                _isLoading = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private string _name;
        public string Name
        {
            get => _name;
            protected set
            {
                _name = value;
                RaiseOnPropertyChanged();
            }
        }

        private string _size;
        public string Size
        {
            get => _size;
            protected set
            {
                _size = value;
                RaiseOnPropertyChanged();
            }
        }
        
        private string _actualSize;
        public string ActualSize
        {
            get => _actualSize;
            protected set
            {
                _actualSize = value;
                RaiseOnPropertyChanged();
            }
        }

        protected Task AsyncTask;
        protected CancellationTokenSource? CancellationSource;
        
        public void CancelTask()
        {
            CancellationSource?.Cancel();
        }
    }
}