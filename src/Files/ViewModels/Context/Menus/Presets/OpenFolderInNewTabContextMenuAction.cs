using System;
using Files.ViewModels.Browser;

namespace Files.ViewModels.Context.Menus.Presets
{
    public sealed class OpenFolderInNewTabContextMenuAction :
        StandardContextMenuItemViewModel
    {
        // TODO: Implement
        public OpenFolderInNewTabContextMenuAction()
        {
            Header = "Open folder in new tab";
            
            //throw new NotImplementedException();
        }

        public override bool MayExecute(BrowserContentViewModelBase? vm)
        {
            return false;
        }
    }
}