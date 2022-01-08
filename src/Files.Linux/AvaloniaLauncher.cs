using Avalonia;
using Avalonia.Controls;

namespace Files.Linux
{
    public class AvaloniaLauncher
    {
        public void Start(string[] args)
        {
            var builder = AppBuilder
                    .Configure<FilesApp>()
                    .UseX11()
                    .UseSkia()
                    .With(new X11PlatformOptions
                    {
                        EnableIme = true,
                        UseDeferredRendering = true,
                        WmClass = "Material.Files"
                    });
            
            builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnLastWindowClose);
        }
    }
}