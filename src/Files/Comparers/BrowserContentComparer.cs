using System.Collections.Generic;
using Files.ViewModels;

namespace Files.Comparers
{
    public abstract class BrowserContentComparer : IComparer<ItemViewModelBase>
    {
        /// <summary>
        /// Get the name of the comparer.
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Get or set the behavior of the comparer. (Ascending or Descending)
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// Get or set the behaviour of the comparer. (Should Folder be before File?)
        /// </summary>
        public bool IsFolderFirst { get; set; } = true;
        
        public abstract int Compare(ItemViewModelBase x, ItemViewModelBase y);
        
        protected abstract int CompareHandler(ItemViewModelBase x, ItemViewModelBase y);
    }
}