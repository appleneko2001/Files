using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Files.Services;
using Files.Services.Android;
using Files.Services.Platform;
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
        internal PlatformSpecificBridge? PlatformApi => _osService?.ApiBridge;

        internal WindowManagerService WindowManager => _windowManager;

        public bool Initialized => _initialized;

        // Fields
        private WindowManagerService _windowManager;
        private OperationSystemService _osService;
        private AppBackend _context;
        private bool _initialized = false;
        
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
        
        public void RegisterOSService(OperationSystemService service)
        {
            if (service == null)
                return;
            
            _osService = service;

            _context.BindServices(service);
        }

        public BrowserWindow CreateBrowserWindow(Uri startUri = null)
        {
            var window = new BrowserWindow(startUri);
            WindowManager.RegisterWindow(window);

            return window;
        }
        
        private void PreInit()
        {
            CommandBase.OnErrorHandler += RelayCommandOnExceptionOccurred;
            
            _windowManager = new WindowManagerService();
            WindowManager.WhenNoActiveWindowsLeft += WhenNoActiveWindowsLeft;
            
            Resources
                .MergedDictionaries
                .Add(new BundledTheme
                {
                    BaseTheme = BaseThemeMode.Dark,
                    PrimaryColor = PrimaryColor.Blue,
                    SecondaryColor = SecondaryColor.Pink
                });

            _context = AppBackend.Instance;
        }

        private void RelayCommandOnExceptionOccurred(object sender, ExecutionFailExceptionArgs e)
        {
            PlatformApi?.PopupMessageWindow("Error", e.Exception.Message);
            
            switch (e.Exception)
            {
                //case UriFormatException:
                //case NotImplementedException:
                case not null:
                    e.Handled = true;
                    break;
            }
        }

        private void PostInit()
        {
            CommandsBackend.Initiate(this);
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