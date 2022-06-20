using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Files.Services.Platform.Interfaces;
using Files.Windows.Services;
using Files.Windows.Services.Platform;

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
                    .AfterSetup(delegate
                    {
                        PostSetup(cancellationToken);
                    });

            builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
        }

        private void PostSetup(CancellationToken cancellationToken)
        {
            var service = new WindowsService(cancellationToken);
            _windowsService = service;

            var apiBridge = new WindowsApiBridge();

            var mutable = AvaloniaLocator.CurrentMutable;

            mutable.Bind<IPlatformSupportDeviceEntries>().ToConstant(apiBridge);
            mutable.Bind<IPlatformSupportOpenFilePrimaryAction>().ToConstant(apiBridge);
            mutable.Bind<IPlatformSupportExecuteApplication>().ToConstant(new ExecuteApplicationHandler());
            mutable.Bind<IPlatformSupportShowMessage>().ToConstant(apiBridge);
            mutable.Bind<IPlatformSupportShowOpenWithDialog>().ToConstant(new OpenWithApplicationHandler());
            //mutable.Bind<IPlatformSupportGetIcon>().ToConstant(apiBridge);
            mutable.Bind<IPlatformSupportNativeExplorer>().ToConstant(apiBridge);

            /*
             
             AvaloniaLocator.CurrentMutable
                .Bind<OperationSystemService>()
                .ToConstant(service);
            if (builder.Instance is FilesApp app)
            {
                app.RegisterOSService(_windowsService);
            }
            */

            //VisualLayerManager.
        }

        internal WindowsService? GetService() => _windowsService;
    }
}