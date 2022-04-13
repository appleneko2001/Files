using MinimalMvvm.ViewModels;

namespace Files.ViewModels.Progress
{
    public class ProgressViewModel : ViewModelBase
    {
        private bool _isComplete;
        private double? _progress;

        public bool IsComplete => _isComplete;
        public bool IsIndeterminate => _progress == null;
        public double Progress => _progress ?? 0.0;

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
    }
}