using System;

namespace Files.Views.Controls.Events
{
    public class AdditionalEventArgs : EventArgs
    {
        public object Argument;
        public EventArgs Source;
    }
}