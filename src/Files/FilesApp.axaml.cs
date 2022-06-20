using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Files.Services;
using Files.Services.Android;
using Files.Services.Platform.Interfaces;
using Files.Views;

using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using MinimalMvvm.Events;
using MinimalMvvm.ViewModels.Commands;

namespace Files
{
    public class FilesApp : Application
    {
        // Events
        public event EventHandler ApplicationInitializationCompleted;
        public event EventHandler ApplicationShutdown;
        
        // Properties

        internal WindowManagerService WindowManager => 
            _windowManager ??= new WindowManagerService();

        public bool Initialized => _initialized;

        // Fields
        private WindowManagerService? _windowManager;
        private bool _initialized;
        
        public override void Initialize()
        {
            PreInit();

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            PostInit();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
            {
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                app.MainWindow = CreateBrowserWindow();

                _initialized = true;
                ApplicationInitializationCompleted(this, EventArgs.Empty);
            }
            
            base.OnFrameworkInitializationCompleted();
        }
        
        /*public void RegisterOSService(OperationSystemService service)
        {
            if (service == null)
                return;
            
            _osService = service;

            _context.BindServices(service);
        }*/

        public BrowserWindow CreateBrowserWindow(Uri? startUri = null)
        {
            var window = new BrowserWindow(startUri);
            WindowManager.RegisterWindow(window);

            return window;
        }
        
        private void PreInit()
        {
            CommandBase.OnErrorHandler += RelayCommandOnExceptionOccurred;
            WindowManager.WhenNoActiveWindowsLeft += WhenNoActiveWindowsLeft;
            
            Resources
                .MergedDictionaries
                .Add(new BundledTheme
                {
                    BaseTheme = BaseThemeMode.Dark,
                    PrimaryColor = PrimaryColor.Blue,
                    SecondaryColor = SecondaryColor.Pink
                });
        }

        private void RelayCommandOnExceptionOccurred(object sender, ExecutionFailExceptionArgs e)
        {
            var service = AvaloniaLocator
                .Current
                .GetService<IPlatformSupportShowMessage>();
            
            service?.PopupMessageWindow("Error", e.Exception.Message);

            e.Handled = e.Exception switch
            {
                //case UriFormatException:
                //case NotImplementedException:
                not null => true,
                _ => e.Handled
            };
        }

        private void PostInit()
        {
            BackgroundTaskBackend.Initiate(this);
            //CommandsBackend.Initiate(this);
            PreviewManagerBackend.Initiate(this);
            ContextMenuBackend.Initiate(this);
            AndroidDebugBackend.Initiate(this);
        }

        private void WhenNoActiveWindowsLeft(object sender, EventArgs e)
        {
            ApplicationShutdown.Invoke(this, EventArgs.Empty);
            
            if (ApplicationLifetime is IControlledApplicationLifetime app)
            {
                app.Shutdown();

                OnApplicationShutdownProperly();
            }
        }

        private void OnApplicationShutdownProperly()
        {

        }
    }
}