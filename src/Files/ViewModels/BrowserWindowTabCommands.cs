using MinimalMvvm.ViewModels.Commands;
// ReSharper disable InconsistentNaming

namespace Files.ViewModels
{
    public partial class BrowserWindowTabViewModel
    {
        private static readonly ExtendedRelayCommand _closeTabCommand =
            new(delegate(object? o)
            {
                if (o is not BrowserWindowTabViewModel vm)
                    return;

                if (vm.CloseTabCommand.MayExecute(vm))
                    OnExecuteCloseCommand(vm);

                vm.CloseTabCommand.RaiseMayExecuteChanged();
            }, delegate(object? o)
            {
                if (o is BrowserWindowTabViewModel vm)
                {
                    return vm.Parent.TabsViewModel.Count > 1;
                }

                return false;
            });

        private static readonly RelayCommand _goBackCommand = new(delegate(object? o)
        {
            if (o is not BrowserWindowTabViewModel vm)
                return;

            if (!vm._tracker.TryPeekPrevious(out var record, true))
                return;

            vm.Open(record!.Uri, false);
        }, o => o is BrowserWindowTabViewModel vm && vm._tracker.CanGoBack);

        private static readonly RelayCommand _goForwardCommand = new(delegate(object? o)
        {
            if (o is not BrowserWindowTabViewModel vm)
                return;

            if (!vm._tracker.TryPeekNext(out var record, true))
                return;

            vm.Open(record!.Uri, false);
        }, o => o is BrowserWindowTabViewModel vm && vm._tracker.CanGoForward);
    }
}