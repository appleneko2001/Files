using System;
using System.Windows.Input;
using Avalonia.Threading;

namespace Files.Commands
{
    public class RelayCommand : ICommand
    {
        public static event EventHandler<OnExecutionOccurExceptionEventArgs>? ExceptionOccur;
        
        private Action<object> _execute;
        private Func<object, bool> _canExecute;
        private event EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove => _canExecuteChanged -= value;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            var result = _canExecute == null || _canExecute(parameter);
            return result;
        }

        public void Execute(object parameter)
        {
            try
            {
                _execute(parameter);
            }
            catch (Exception e)
            {
                var args = new OnExecutionOccurExceptionEventArgs
                {
                    Exception = e
                };
                ExceptionOccur?.Invoke(this, args);

                if (!args.ShouldKeepAppAlive)
                    throw new AggregateException(args.Exception);
            }
        }

        // Call this method to tell AvaloniaUI about this command can be executed at this moment.
        public void RaiseCanExecute()
        {
            // Call CanExecute via Dispatcher.UIThread.Post to prevent CanExecute can't be called from other thread.
            Dispatcher.UIThread.Post(delegate
            {
                _canExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        } 
    }
}