using System;

namespace Files.Commands
{
    public class OnExecutionOccurExceptionEventArgs : EventArgs
    {
        public Exception Exception;
        
        public bool ShouldKeepAppAlive;
    }
}