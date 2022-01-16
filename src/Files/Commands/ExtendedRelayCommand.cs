using System;
using System.Windows.Input;
using Avalonia.Threading;

namespace Files.Commands
{
    /// <summary>
    /// <p>RelayCommand, but extended a may execute interface.</p>
    /// <p>Idea from <a href="https://github.com/brandonhood">github@brandonhood</a></p>
    /// </summary>
    public class ExtendedRelayCommand : ICommand, IMayExecuteCommand
    {
        public static event EventHandler<OnExecutionOccurExceptionEventArgs>? ExceptionOccur;
        
        private Action<object> _execute;
        private Func<object, bool> _mayExecute;
        private Func<object, bool> _canExecute;
        private event EventHandler _canExecuteChanged;
        private event EventHandler _mayExecuteChanged;
        
        public event EventHandler CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove => _canExecuteChanged -= value;
        }
        
        public event EventHandler MayExecuteChanged
        {
            add => _mayExecuteChanged += value;
            remove => _mayExecuteChanged -= value;
        }

        public ExtendedRelayCommand(Action<object> execute, Func<object, bool> canExecute = null, Func<object, bool> mayExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _mayExecute = mayExecute;
        }
        
        public bool MayExecute(object parameter)
        {
            var result = _mayExecute == null || _mayExecute(parameter);
            return result;
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
        
        // Call this method to tell AvaloniaUI about this command can be executed at this moment.
        public void RaiseMayExecute()
        {
            // Call CanExecute via Dispatcher.UIThread.Post to prevent CanExecute can't be called from other thread.
            Dispatcher.UIThread.Post(delegate
            {
                _mayExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}