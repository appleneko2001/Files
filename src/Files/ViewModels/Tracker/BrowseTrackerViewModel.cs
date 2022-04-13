using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Tracker
{
    public class BrowseTrackerViewModel : ViewModelBase
    {
        private ObservableCollection<BrowseTrackerRecordElement> _stack;
        private const int MaxTrackingCount = 64;

        private BrowseTrackerRecordElement _currentRecord;

        public BrowseTrackerRecordElement CurrentRecord
        {
            get => _currentRecord;
            private set
            {
                _currentRecord = value;
                OnPropertyChanged();
            }
        }

        public INotifyCollectionChanged ObservableStack => _stack;

        public bool CanGoBack => _stack.Count > 0;

        public BrowseTrackerViewModel()
        {
            _stack = new ObservableCollection<BrowseTrackerRecordElement>();
        }

        public void PushAndSetCurrent(BrowseTrackerRecordElement element)
        {
            if(_stack.Count >= MaxTrackingCount)
                _stack.RemoveAt(0);
            
            _stack.Add(element);
            CurrentRecord = element;
        }

        public bool TryPopAndSetCurrent(out BrowseTrackerRecordElement? element)
        {
            element = null;

            // Return false if stack is empty
            if (_stack.Count == 0)
                return false;

            var last = _stack.Last();
            _stack.Remove(last);

            element = last;
            CurrentRecord = element;
            return true;
        }
    }
}