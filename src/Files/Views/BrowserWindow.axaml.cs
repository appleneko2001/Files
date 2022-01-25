﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
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
using Files.Extensions;
using Files.Services;
using Files.Views.Models;
using Material.Colors;
using Material.Styles;
using Material.Styles.Controls;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Files.Views
{
    public class BrowserWindow : Window
    {
        private static bool _isAppliedInitialTheme = false;
        //public const string SnackbarHost = "RootSnackbarHost";

        private readonly BrowserWindowViewModel _context;

        // ReSharper disable InconsistentNaming
        // Control references
        private Grid PART_BrowserViewRoot;
        private ColorZone PART_ContentViewColorZone;
        private ColorZone PART_AppbarColorZone;

        // ReSharper restore InconsistentNaming

        // Private fields -- object instance
        private double _backgroundBrightness = 0.2;
        private Bitmap? _previousBackgroundBitmap;

        private static readonly PaletteHelper _paletteHelper = new();

        private static readonly BundledTheme _lightTheme = new()
        {
            BaseTheme = BaseThemeMode.Light,
            PrimaryColor = PrimaryColor.Indigo,
            SecondaryColor = SecondaryColor.Pink
        };

        private static readonly BundledTheme _darkTheme = new()
        {
            BaseTheme = BaseThemeMode.Dark,
            PrimaryColor = PrimaryColor.LightBlue,
            SecondaryColor = SecondaryColor.Pink
        };

        // The application will not be compiled if this class implemented without default constructor
        // ReSharper disable once UnusedMember.Global
        public BrowserWindow()
        {
            InitializeComponent();
        }

        public BrowserWindow(Uri? startUri = null)
        {
            InitializeComponent();

            #if DEBUG
            Avalonia.Diagnostics.DevTools.Attach(this, KeyGesture.Parse("F12"));
            #endif

            var mainContext = AppBackend.Instance;
            _context = new BrowserWindowViewModel(mainContext, this);
            DataContext = _context;

            PART_BrowserViewRoot!.DataContext = _context;

            if (!_isAppliedInitialTheme)
            {
                _isAppliedInitialTheme = true;
                _paletteHelper.SetTheme(_darkTheme.GetTheme());
            }

            if (startUri is not null)
                _context.SelectedTab.Open(startUri);
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

            if (bitmap.PixelSize.GreaterThan(new PixelSize(2560, 1440)))
            {
                AppBackend.Instance.ShowNativeDialog("Warning",
                    "The background picture size is bigger than 2560x1440, the performance impact could issued.");
            }

            var brush = new ImageBrush(bitmap)
            {
                BitmapInterpolationMode = BitmapInterpolationMode.HighQuality,
                Stretch = Stretch.UniformToFill,
                Opacity = _backgroundBrightness
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

        private Dictionary<string, IDisposable>? _disposables;

        private void OnScrollerAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is Scroller scroller)
            {
                if (_disposables == null)
                {
                    _disposables = new();
                }

                _disposables.Add(scroller.Name, scroller.GetObservable(OpacityMaskProperty).Subscribe(delegate
                {
                    scroller.InvalidateVisual();
                }));
            }
        }

        private void OnScrollerDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is Scroller scroller)
            {
                var name = scroller.Name;
                if (_disposables.TryGetValue(name, out var disposable))
                {
                    disposable.Dispose();
                    _disposables.Remove(name);
                }

                if (_disposables.Count == 0)
                {
                    _disposables.Clear();
                    _disposables = null;
                }
            }
        }

        private void BackgroundDarknessOpacitySlider_OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Value" && e.NewValue is double value)
            {
                _backgroundBrightness = value;
                
                if (Background is not ImageBrush brush)
                    return;

                brush.Opacity = _backgroundBrightness;
            }
        }
    }
}