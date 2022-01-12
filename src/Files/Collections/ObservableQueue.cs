using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Files.Collections
{
    public class ObservableQueue<T> : Queue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new virtual T Dequeue()
        {
            if (Count == 0)
                return default(T);

            var item = base.Dequeue();//Pop();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }

        public new virtual void Enqueue(T item)
        {
            base.Enqueue(item);//Push(item);
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