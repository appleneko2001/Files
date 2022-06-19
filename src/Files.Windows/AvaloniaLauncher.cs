using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Files.Windows.Services;

namespace Files.Windows
{
    public class AvaloniaLauncher
    {
        private WindowsService? _windowsService;
        
        public void Start(string[] args, CancellationToken cancellationToken)
        {
            var builder = AppBuilder
                    .Configure<FilesApp>()
                    .UseWin32()
                    .UseSkia()
                    .With(new Win32PlatformOptions
                    {
                        UseWindowsUIComposition = true,
                        AllowEglInitialization = true,
                        UseDeferredRendering = true,
                        EnableMultitouch = true
                    })
                    .AfterSetup(delegate(AppBuilder appBuilder)
                    {
                        PostSetup(appBuilder, cancellationToken);
                    });

            builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
        }

        private void PostSetup(AppBuilder builder, CancellationToken cancellationToken)
        {
            _windowsService = new WindowsService(cancellationToken);
            
            if (builder.Instance is FilesApp app)
            {
                app.RegisterOSService(_windowsService);
            }
            
            //VisualLayerManager.
        }

        internal WindowsService? GetService() => _windowsService;
    }
}