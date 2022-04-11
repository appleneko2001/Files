using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Files.Extensions
{
    public static class ListExtensions
    {
        // I dont sure why copilot want this lmao
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
        }

        public static void SortAdd<T>(this Collection<T> collection, IComparer<T> comparer, T element)
        {
            collection.Add(element);

            var i = collection.Count - 1;
            for (; i > 0 && comparer.Compare(collection[i - 1], element) < 0; i--)
            {
                collection[i] = collection[i - 1];
            }

            collection[i] = element;
        }
    }
}