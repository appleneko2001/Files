using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Files.Collections
{
    public class ObservableQueue<T> : Queue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ManualResetEventSlim _removeEvent = new (false);
        
        public ObservableQueue()
        {
        }

        public ObservableQueue(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                base.Enqueue(item);//Push(item);
        }

        public ObservableQueue(List<T> list)
        {
            foreach (var item in list)
                base.Enqueue(item);
        }

        public new virtual void Clear()
        {
            base.Clear();
            
            if(!_removeEvent.IsSet)
                _removeEvent.Set();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new virtual T Dequeue()
        {
            if (Count == 0)
                throw new IndexOutOfRangeException("Queue is empty");

            var item = base.Dequeue();
            
            if(!_removeEvent.IsSet)
                _removeEvent.Set();
            
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }

        public new virtual void Enqueue(T item)
        {
            // Might occur a exception if queue is full
            try
            {
                base.Enqueue(item);
            }
            catch (ArgumentException)
            {
                _removeEvent.Wait();
                _removeEvent.Reset();
                
                base.Enqueue(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChanged(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e);
        }

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        private void RaisePropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }
    }
}