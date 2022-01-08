using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace Files.Views.Models
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseOnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected void RaiseOnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            PropertyChanged?.Invoke(this, eventArgs);
        }
        
        protected void RaiseOnPropertyChangedThroughUiThread([CallerMemberName] string propertyName = null, DispatcherPriority priority = DispatcherPriority.Background)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }, priority);
        }
        
        protected void RaiseOnPropertyChangedThroughUiThread(PropertyChangedEventArgs eventArgs, DispatcherPriority priority = DispatcherPriority.Background)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                PropertyChanged?.Invoke(this, eventArgs);
            }, priority);
        }
    }
}