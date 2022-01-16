using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Files.Commands;
using Files.Services;
using Files.Views;

using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Files
{
    public class FilesApp : Application
    {
        // Events
        public event EventHandler ApplicationInitializationCompleted;
        public event EventHandler ApplicationShutdown;
        
        // Properties
        internal PlatformSpecificBridge? PlatformApi => _osService?.ApiBridge;
        
        // Fields
        private WindowManagerService _windowManager;
        private OperationSystemService _osService;
        private AppBackend _context;
        
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

        public BrowserWindow CreateBrowserWindow()
        {
            var window = new BrowserWindow();
            _windowManager.RegisterWindow(window);

            return window;
        }
        
        private void PreInit()
        {
            RelayCommand.ExceptionOccur += RelayCommandOnExceptionOccurred;
            ExtendedRelayCommand.ExceptionOccur += RelayCommandOnExceptionOccurred;
            
            _windowManager = new WindowManagerService();
            _windowManager.WhenNoActiveWindowsLeft += WhenNoActiveWindowsLeft;
            
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

        private void PostInit()
        {
            CommandsBackend.Initiate(this);
            PreviewManagerBackend.Initiate(this);
            ContextMenuBackend.Initiate(this);
        }

        private void RelayCommandOnExceptionOccurred(object sender, OnExecutionOccurExceptionEventArgs e)
        {
            PlatformApi?.PopupMessageWindow("Error", e.Exception.Message);
            
            switch (e.Exception)
            {
                //case UriFormatException:
                //case NotImplementedException:
                case not null:
                    e.ShouldKeepAppAlive = true;
                    break;
            }
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