using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Files.Services;
using Files.Views.Models;

using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Files.Views
{
    public class BrowserWindow : Window
    {
        //public const string SnackbarHost = "RootSnackbarHost";

        private readonly BrowserWindowViewModel _context;
        
        // ReSharper disable InconsistentNaming
        // Control references
        private Grid PART_BrowserViewRoot;
        private ColorZone PART_ContentViewColorZone;
        private ColorZone PART_AppbarColorZone;
        
        // ReSharper restore InconsistentNaming

        private readonly PaletteHelper _paletteHelper = new();

        private readonly BundledTheme _lightTheme = new()
        {
            BaseTheme = BaseThemeMode.Light,
            PrimaryColor = PrimaryColor.Indigo,
            SecondaryColor = SecondaryColor.Pink
        };
        
        private readonly BundledTheme _darkTheme = new()
        {
            BaseTheme = BaseThemeMode.Dark,
            PrimaryColor = PrimaryColor.LightBlue,
            SecondaryColor = SecondaryColor.Pink
        };
        
        public BrowserWindow()
        {
            InitializeComponent();

            Avalonia.Diagnostics.DevTools.Attach(this, KeyGesture.Parse("F12"));

            var mainContext = AppBackend.Instance;
            _context = new BrowserWindowViewModel(mainContext, this);
            DataContext = _context;

            PART_BrowserViewRoot!.DataContext = _context;
            
            
            _paletteHelper.SetTheme(_darkTheme.GetTheme());
        }

        protected override void HandleWindowStateChanged(WindowState state)
        {
            CanResize = state is not (WindowState.Maximized or WindowState.FullScreen);

            base.HandleWindowStateChanged(state);
        }

        /// <summary>
        /// Preload visual source code. Internal use.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PART_BrowserViewRoot = this.Get<Grid>(nameof(PART_BrowserViewRoot));
            PART_ContentViewColorZone = this.Get<ColorZone>(nameof(PART_ContentViewColorZone));
            PART_AppbarColorZone = this.Get<ColorZone>(nameof(PART_AppbarColorZone));
        }

        private void NavDrawerButton_OnClick(object sender, RoutedEventArgs e)
        {
            _context.IsNavigationDrawerOpen = !_context.IsNavigationDrawerOpen;
        }

        private void RootAppBar_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (WindowState == WindowState.FullScreen)
                return;
            
            BeginMoveDrag(e);
        }

        private void SwitchThemeButton_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(delegate
            {
                var theme = _paletteHelper.GetTheme();
                var baseTheme = theme.GetBaseTheme();

                switch (baseTheme)
                {
                    case BaseThemeMode.Light:
                        _paletteHelper.SetTheme(_darkTheme.GetTheme());
                        break;

                    case BaseThemeMode.Dark:
                    default:
                        _paletteHelper.SetTheme(_lightTheme.GetTheme());
                        break;
                }
            });
        }
        
        
    }
}