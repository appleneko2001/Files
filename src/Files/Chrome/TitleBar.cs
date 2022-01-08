using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.VisualTree;
using Material.Styles;

namespace Files.Chrome
{
    [PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen", ":left-aligned-buttons", ":right-aligned-buttons")]
    public class TitleBar : TemplatedControl
    {
        private CompositeDisposable? _disposables;
        private TitleBarCaptionButtons? _captionButtons;
        private Border? _dragzone;
        private DateTime _prevClickDragzone;


        public static readonly StyledProperty<string> TitleProperty =
            Window.TitleProperty.AddOwner<TitleBar>();

        public string Title
        {
            get => GetValue(TitleProperty);
            private set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<ColorZoneMode> ModeProperty =
            AvaloniaProperty.Register<TitleBar, ColorZoneMode>(nameof(Mode));

        public ColorZoneMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }
        
        public static readonly StyledProperty<CaptionButtonsAlignment> CaptionButtonsAlignProperty =
            AvaloniaProperty.Register<TitleBar, CaptionButtonsAlignment>(nameof(CaptionButtonsAlign));

        public CaptionButtonsAlignment CaptionButtonsAlign
        {
            get => GetValue(CaptionButtonsAlignProperty);
            set => SetValue(CaptionButtonsAlignProperty, value);
        }

        private void UpdateSize(Window window)
        {
            if (window != null)
            {
                IsVisible = window.PlatformImpl.NeedsManagedDecorations;
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _captionButtons?.Detach();
            
            _captionButtons = e.NameScope.Get<TitleBarCaptionButtons>("PART_CaptionButtons");

            if (VisualRoot is Window window)
            {
                _captionButtons?.Attach(window);   
                
                UpdateSize(window);
            }

            try
            {
                _dragzone = e.NameScope.Get<Border>("PART_DragZone");
                if (_dragzone != null)
                {
                    _dragzone.PointerPressed += DragzonePointerPressed;
                    _dragzone.PointerReleased += DragzonePointerReleased;
                }
            }
            catch
            {
                // ignored
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (VisualRoot is Window window)
            {
                _disposables = new CompositeDisposable
                {
                    window.GetObservable(Window.WindowDecorationMarginProperty)
                        .Subscribe(delegate { UpdateSize(window); }),
                    window.GetObservable(Window.ExtendClientAreaTitleBarHeightHintProperty)
                        .Subscribe(delegate { UpdateSize(window); }),
                    window.GetObservable(Window.OffScreenMarginProperty)
                        .Subscribe(delegate { UpdateSize(window); }),
                    window.GetObservable(Window.ExtendClientAreaChromeHintsProperty)
                        .Subscribe(delegate { UpdateSize(window); }),
                    window.GetObservable(Window.WindowStateProperty)
                        .Subscribe(delegate(WindowState x)
                        {
                            PseudoClasses.Set(":minimized", x == WindowState.Minimized);
                            PseudoClasses.Set(":normal", x == WindowState.Normal);
                            PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                            PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                        }),
                    window.GetObservable(Window.IsExtendedIntoWindowDecorationsProperty)
                        .Subscribe(delegate { UpdateSize(window); }),
                    window.GetObservable(Window.TitleProperty).Subscribe(delegate(string s)
                    {
                        Title = s;
                    }),
                    
                    
                    this.GetObservable(CaptionButtonsAlignProperty).Subscribe(
                        delegate(CaptionButtonsAlignment alignment)
                        {
                            PseudoClasses.Set(":left-aligned-buttons", alignment == CaptionButtonsAlignment.Left);
                            PseudoClasses.Set(":right-aligned-buttons", alignment == CaptionButtonsAlignment.Right);
                        }),
                };

                var visualLayerManagers = window.GetTemplateChildren().Where(delegate(IVisual visual)
                {
                    return visual is VisualLayerManager {Name: "PART_VisualLayerManagerRoot"};
                });
                if (visualLayerManagers.Any())
                {
                    var v = visualLayerManagers.First() as VisualLayerManager;
                    if(v?.ChromeOverlayLayer != null)
                        v.ChromeOverlayLayer.ZIndex = 100;
                }
            }
        }

        private void DragzonePointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (VisualRoot is Window window)
            {
                if (window.WindowState == WindowState.FullScreen)
                    return;
                
                window.BeginMoveDrag(e);
            }
        }
        
        private void DragzonePointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (VisualRoot is Window window)
            {
                var delta = DateTime.Now - _prevClickDragzone;

                // If delta is less than 300ms
                if (delta.TotalSeconds < 0.3)
                {
                    window.WindowState = window.WindowState == WindowState.Normal
                        ? WindowState.Maximized
                        : WindowState.Normal;
                }
            
                _prevClickDragzone = DateTime.Now;
            }
        }
        
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (_dragzone != null)
            {
                _dragzone.PointerPressed -= DragzonePointerPressed;
                _dragzone.PointerReleased -= DragzonePointerReleased;
                _dragzone = null;
            }
            
            base.OnDetachedFromVisualTree(e);

            _disposables?.Dispose();
            
            _captionButtons?.Detach();
            _captionButtons = null;
        }
    }
}