using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Visuals.Media.Imaging;

using Files.Services;
using Files.Views.Models;

using Material.Colors;
using Material.Styles;
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
        
        // Private fields -- object instance
        private Bitmap? _previousBackgroundBitmap;

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
        
        private async void UseBackgroundImageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new()
                    {
                        Name = "Picture file",
                        Extensions = new List<string>
                        {
                            "png", "bmp", "jpg", "jpeg"
                        }
                    },
                    new()
                    {
                        Name = "All file types (not recommended)",
                        Extensions = new List<string>
                        {
                            "*"
                        }
                    }
                }
            };
            var result = await dialog.ShowAsync(this);
            if (result is null)
                return;

            if (!result.Any())
                return;
            
            await using var stream = new FileStream(result.First(), FileMode.Open, FileAccess.Read);

            var bitmap = new Bitmap(stream);
            
            var brush = new ImageBrush(bitmap)
            {
                BitmapInterpolationMode = BitmapInterpolationMode.HighQuality,
                Stretch = Stretch.UniformToFill,
                Opacity = 0.3 
            };

            Background = brush;

            PART_AppbarColorZone.Background = Brushes.Transparent;
            PART_ContentViewColorZone.Background = Brushes.Transparent;

            DisposeBackgroundImage();
            
            _previousBackgroundBitmap = bitmap;
        }

        private void UseDefaultBackgroundButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current is not IResourceHost host)
                throw new NullReferenceException("Unable to get resource host.");

            if (!host.HasResources || !host.TryGetResource("MaterialDesignPaper", out _)) 
                return;

            var source = host.GetResourceObservable("MaterialDesignPaper");

            DelayedBinding.Add(this, BackgroundProperty, source.ToBinding());
            
            DisposeBackgroundImage();
        }

        private void DisposeBackgroundImage()
        {
            if (_previousBackgroundBitmap == null)
                return;
            
            _previousBackgroundBitmap.Dispose();
            _previousBackgroundBitmap = null;
        }
    }
}