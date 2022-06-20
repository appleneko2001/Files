using System.Collections.Generic;
using Files.ViewModels;
using Files.ViewModels.Browser;

namespace Files.Models.Actions
{
    public class BrowserActionParameterModel
    {
        public ItemViewModelBase? SelectedItem;
        public IReadOnlyList<ItemViewModelBase>? SelectedItems;
        public BrowserContentViewModelBase BrowserViewModel;
    }
}