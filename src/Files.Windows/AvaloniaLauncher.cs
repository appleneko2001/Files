using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Files.Windows.Services;

namespace Files.Windows
{
    public class AvaloniaLauncher
    {
        private WindowsService windowsService;
        
        public void Start(string[] args, CancellationToken cancellationToken)
        {
            var builder = AppBuilder
                    .Configure<FilesApp>()
                    .UseWin32()
                    .UseSkia()
                    .With(new Win32PlatformOptions
                    {
                        OverlayPopups = true,
                        UseWindowsUIComposition = false,
                        AllowEglInitialization = true,
                        UseDeferredRendering = true
                    })
                    .AfterSetup(delegate(AppBuilder appBuilder)
                    {
                        PostSetup(appBuilder, cancellationToken);
                    });

            builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
        }

        private void PostSetup(AppBuilder builder, CancellationToken cancellationToken)
        {
            windowsService = new WindowsService(cancellationToken);
            
            if (builder.Instance is FilesApp app)
            {
                app.RegisterOSService(windowsService);
            }
            
            //VisualLayerManager.
        }

        internal WindowsService GetService() => windowsService;
    }
}