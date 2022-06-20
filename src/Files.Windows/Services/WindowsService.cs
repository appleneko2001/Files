using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Files.Services.Platform.Interfaces;
using Files.Windows.Services.Native;
using Files.Windows.Services.Watchers;

namespace Files.Windows.Services
{
    public class WindowsService
    {
        private readonly ServiceWindow _nativeServiceInst;
        private readonly CancellationToken _cancellationToken;

        private IntPtr _messageLoopThreadId;

        public WindowsService(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            var longTaskThread = new Thread(OnUpdate)
            {
                Name = "Windows Message Receiver Thread",
                Priority = ThreadPriority.BelowNormal
            };
            
            var instance = Marshal.GetHINSTANCE(GetType().Module);
            _nativeServiceInst = new ServiceWindow(instance);

            var deviceWatcher = new WindowsDeviceWatcher(_nativeServiceInst);

            AvaloniaLocator.CurrentMutable.Bind<IPlatformSupportDeviceWatcherService>().ToConstant(deviceWatcher);
            
            longTaskThread.Start();
        }
        
        private void OnUpdate()
        {
            _messageLoopThreadId = new IntPtr(NativeApi.GetCurrentThreadId());
            try
            {
                while (_nativeServiceInst.PullEvent(_cancellationToken))
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException canceledException)
            {
                if (canceledException.CancellationToken != _cancellationToken)
                    throw;
                
                Stop();
            }
        }

        private void Stop()
        {
            // ReSharper disable once ConstantConditionalAccessQualifier
            //_deviceWatcher.Stop();
            
            _nativeServiceInst.Dispose();
        }

        // Used for break message-loop waiting lock.
        public void SendEmptyMessage()
        {
            if (!NativeApi.PostThreadMessage(_messageLoopThreadId, MessageDefinitions.UserNullMessage,
                    IntPtr.Zero, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}