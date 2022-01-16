using System;

namespace Files.Commands
{
    public interface IMayExecuteCommand
    {
        event EventHandler MayExecuteChanged;

        bool MayExecute(object parameter);
    }
}