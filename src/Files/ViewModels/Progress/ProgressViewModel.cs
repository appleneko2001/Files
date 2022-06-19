using System.Threading;
using MinimalMvvm.ViewModels;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Progress
{
    public class ProgressViewModel : ViewModelBase
    {
        private static readonly RelayCommand _cancelCommand = new (CancelCommand_Execute);

        private static void CancelCommand_Execute(object? obj)
        {
            if (obj is not ProgressViewModel vm)
                return;
            
            vm.TryCancel();
        }


        private bool _isComplete;
        private double? _progress;
        private readonly CancellationTokenSource? _cancellationTokenSource;

        public ProgressViewModel(CancellationTokenSource? cancellationTokenSource = default)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        public bool IsComplete => _isComplete;
        public bool IsIndeterminate => _progress == null;
        public double Progress => _progress ?? 0.0;
        public RelayCommand CancelCommand => _cancelCommand;

        public void SetCompleted()
        {
            _isComplete = true;
            OnPropertyChanged(nameof(IsComplete));
        }

        public void SetProgress(double? p)
        {
            _progress = p;
            OnPropertyChanged(nameof(IsIndeterminate));
            OnPropertyChanged(nameof(Progress));
        }

        public void TryCancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}