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
                            ? string.Compare(xFsItem.Name, yFsItem.Name,
                                StringComparison.Ordinal)
                            : xFsItem.IsFolder
                                ? -1
                                : yFsItem.IsFolder
                                    ? 1
                                    : string.Compare(xFsItem.Name, yFsItem.Name, StringComparison.Ordinal),
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
    }
}