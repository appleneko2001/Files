using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Avalonia.Threading;

namespace Files.Extensions
{
    public static class ListExtensions
    {
        // I don't sure why copilot want this lmao
        /*
        public static List<T> Shuffle<T>(this List<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }*/

        public static void SortAdd<T>(this Collection<T> collection, IComparer<T> comparer, T element)
        {
            var i = SortCore(collection, comparer, element);
            
            collection.Insert(i, element);
        }
        
        public static void SortAddOnUiThread<T>(this Collection<T> collection, IComparer<T> comparer, T element)
        {
            var i = SortCore(collection, comparer, element);

            Dispatcher.UIThread.InvokeAsync(delegate
            {
                collection.Insert(i, element);
            }, DispatcherPriority.Background).Wait();

            if (Dispatcher.UIThread.HasJobsWithPriority(DispatcherPriority.Background))
                Thread.Sleep(1);
        }

        private static int SortCore<T>(IReadOnlyList<T> collection, IComparer<T> comparer, T element)
        {
            if (collection.Count == 0)
                return 0;
            
            var i = collection.Count;

            while (i > 0 && comparer.Compare(collection[i - 1], element) < 0)
            {
                i--;
            }

            return i;
        }
    }
}