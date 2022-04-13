using System.Threading;
using System.Threading.Tasks;
using Files.ViewModels.Interfaces;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Browser.Properties
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
                OnPropertyChanged();
            }
        }
        
        private string _name;
        public string Name
        {
            get => _name;
            protected set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _size;
        public string Size
        {
            get => _size;
            protected set
            {
                _size = value;
                OnPropertyChanged();
            }
        }
        
        private string _actualSize;
        public string ActualSize
        {
            get => _actualSize;
            protected set
            {
                _actualSize = value;
                OnPropertyChanged();
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