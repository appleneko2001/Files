using System;
using Files.ViewModels;
using Files.ViewModels.Browser.Files;

namespace Files.Comparers
{
    public class SortByNameComparer : BrowserContentComparer
    {
        public override string Name => "Sort by Name";

        public override int Compare(ItemViewModelBase x, ItemViewModelBase y)
        {
            var result = CompareHandler(x, y);
            return Reverse ? result : result * -1;
        }

        protected override int CompareHandler(ItemViewModelBase? x, ItemViewModelBase? y)
        {
            switch (x)
            {
                case FileSystemItemViewModel xFsItem:
                    return y switch
                    {
                        FileSystemItemViewModel yFsItem => xFsItem.IsFolder && yFsItem.IsFolder || !IsFolderFirst
                            ? CompareByName(xFsItem, yFsItem)
                            : xFsItem.IsFolder
                                ? -1
                                : yFsItem.IsFolder
                                    ? 1
                                    : CompareByName(xFsItem, yFsItem),
                        _ => -1
                    };

                case null when y == null:
                    return 0;
                case null:
                    return -1;
                default:
                {
                    if (y == null)
                    {
                        return 1;
                    }

                    break;
                }
            }

            return 0;
        }
        
        private static int CompareByName(ItemViewModelBase x, ItemViewModelBase y) => 
            string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }
}