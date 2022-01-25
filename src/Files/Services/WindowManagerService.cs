using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Files.Views;

namespace Files.Services
{
    public class WindowManagerService
    {
        public event EventHandler WhenNoActiveWindowsLeft;
        
        private ObservableCollection<Window> _activeWindows;
        private Window? _lastFocusedWindow;
        private bool _isLockedDown;
        
        public Window? LastFocusedWindow => _lastFocusedWindow;
        
        public WindowManagerService()
        {
            _activeWindows = new ObservableCollection<Window>();
        }

        public bool RegisterWindow(Window inst)
        {
            if (inst is null || _isLockedDown)
                return false;

            if (_activeWindows.Contains(inst))
                return false;
            
            inst.GotFocus += OnAvaloniaWindowGotFocus;
            inst.Closed += OnAvaloniaWindowClosed;
            _activeWindows.Add(inst);
            return true;
        }

        private void OnAvaloniaWindowGotFocus(object sender, GotFocusEventArgs e)
        {
            if(sender is not BrowserWindow window)
                return;

            _lastFocusedWindow = window;
        }

        private void OnAvaloniaWindowClosed(object sender, EventArgs e)
        {
            if (sender is Window window)
            {
                window.GotFocus -= OnAvaloniaWindowGotFocus;
                _activeWindows.Remove(window);
            }

            if (_activeWindows.Count <= 0)
            {
                WhenNoActiveWindowsLeft?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}