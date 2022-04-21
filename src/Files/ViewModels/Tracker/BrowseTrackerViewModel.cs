using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Tracker
{
    public class BrowseTrackerViewModel : ViewModelBase
    {
        private readonly ObservableCollection<BrowseTrackerRecordElement> _stack;
        private const int MaxTrackingCount = 64;

        private BrowseTrackerRecordElement? _currentRecord;

        // ReSharper disable once MemberCanBePrivate.Global
        public BrowseTrackerRecordElement? CurrentRecord
        {
            get => _currentRecord;
            private set
            {
                _currentRecord = value;
                OnPropertyChanged();
            }
        }

        public INotifyCollectionChanged ObservableStack => _stack;

        public bool CanGoBack => _stack.Count > 1 && _stack.First() != _currentRecord;
        
        public bool CanGoForward => _stack.Count > 1 && _stack.Last() != _currentRecord;

        public BrowseTrackerViewModel()
        {
            _stack = new ObservableCollection<BrowseTrackerRecordElement>();
        }

        public void PushAndSetCurrent(BrowseTrackerRecordElement element, bool removeStacks = true)
        {
            if (removeStacks)
            {
                var c = CurrentRecord;

                for (var j = _stack.Count - 1; j >= 0; j--)
                {
                    var i = _stack[j];
                    
                    if (c == i)
                        break;
                        
                    _stack.RemoveAt(j);
                }
            }
            
            if(_stack.Count >= MaxTrackingCount)
                _stack.RemoveAt(0);
            
            _stack.Add(element);
            CurrentRecord = element;
        }
        
        public bool TryPeekPrevious(out BrowseTrackerRecordElement? element, bool setCurrent = false)
        {
            if(CurrentRecord == null)
            {
                element = null;
                return false;
            }
            
            if (_stack.Count > 1)
            {
                var index = _stack.IndexOf(CurrentRecord) - 1;
                
                if(index >= 0)
                {
                    element = _stack[index];
                    
                    if(setCurrent)
                        CurrentRecord = element;
                    
                    return true;
                }
            }

            element = null;
            return false;
        }
        
        public bool TryPeekNext(out BrowseTrackerRecordElement? element, bool setCurrent = false)
        {
            if(CurrentRecord == null)
            {
                element = null;
                return false;
            }
            
            var c = CurrentRecord;
            
            if (_stack.Count > 0)
            {
                var index = _stack.IndexOf(c);
                if (index < _stack.Count - 1)
                {
                    element = _stack[index + 1];
                    
                    if(setCurrent)
                        CurrentRecord = element;
                    
                    return true;
                }
            }

            element = null;
            return false;
        }
        
        // ReSharper disable once UnusedMember.Global
        public bool TryPopAndSetCurrent(out BrowseTrackerRecordElement? element)
        {
            var result = TryPop(out element);
            CurrentRecord = element;

            return result;
        }
        
        public IEnumerable<BrowseTrackerRecordElement> GetBackList()
        {
            var list = new List<BrowseTrackerRecordElement>();
            
            if(CurrentRecord == null)
            {
                return list;
            }
            
            var c = CurrentRecord;
            var index = _stack.IndexOf(c);

            for (var i = index; i >= 0; i--)
            {
                list.Add(_stack[i]);
            }

            return list;
        }

        private bool TryPop(out BrowseTrackerRecordElement? element)
        {
            element = null;

            // Return false if stack is empty
            if (_stack.Count == 0)
                return false;

            var last = _stack.Last();
            _stack.Remove(last);

            element = last;
            return true;
        }
    }
}