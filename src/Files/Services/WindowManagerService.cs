using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace Files.Services
{
    public class WindowManagerService
    {
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
            
            inst.Closed += OnAvaloniaWindowClosed;
            _activeWindows.Add(inst);
            return true;
        }

        public event EventHandler WhenNoActiveWindowsLeft;

        private void OnAvaloniaWindowClosed(object sender, EventArgs e)
        {
            if (sender is Window window)
            {
                _activeWindows.Remove(window);
            }

            if (_activeWindows.Count <= 0)
            {
                WhenNoActiveWindowsLeft?.Invoke(this, EventArgs.Empty);
            }
        }

        private ObservableCollection<Window> _activeWindows;
        private bool _isLockedDown;
    }
}