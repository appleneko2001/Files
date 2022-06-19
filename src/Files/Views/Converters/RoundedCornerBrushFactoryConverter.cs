using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Files.Views.Converters.Parameters;

namespace Files.Views.Converters
{
    public class RoundedCornerBrushInstance
    {
        public IBitmap Bitmap;
        public Rect ControlSize;

        public RoundedCornerBrushInstance(IBitmap bitmap, Rect size)
        {
            Bitmap = bitmap;
            ControlSize = size;
        }
    }
    
    public class RoundedCornerBrushFactoryConverter : IValueConverter
    {
        private static Dictionary<string, RoundedCornerBrushInstance> _reusePool = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Rect rect)
                throw new ArgumentException(
                    $"Invalid object type. Expected Avalonia.Rect. Actual: {value?.GetType().FullName ?? "null"}",
                    nameof(value));

            if (rect.IsEmpty || rect.Width == 0 || rect.Height == 0)
                return null;

            if (parameter is not RoundedCornerBrushFactoryParameter args)
                throw new ArgumentException(
                    $"Invalid object type. Expected {typeof(RoundedCornerBrushFactoryParameter)}. Actual: {parameter?.GetType().FullName ?? "null"}",
                    nameof(parameter));

            if (!string.IsNullOrEmpty(args.ReuseId))
            {
                if (_reusePool.TryGetValue(args.ReuseId, out var inst))
                {
                    if (args.UpdateWhenSizeChanged)
                    {
                        if (inst.ControlSize == rect)
                            return inst.Bitmap;

                        var geometry = GenerateRoundedCornerGeometry(rect, args.CornerRadius);
                        inst.Bitmap = ReDrawBitmapInstance(inst.Bitmap, geometry, args.Brush);
                        inst.ControlSize = rect;

                        return inst.Bitmap;
                    }
                }
            }

            {
                var geometry = GenerateRoundedCornerGeometry(rect, args.CornerRadius);
                var newInstance = InitiateBitmapInstance(geometry, args.Brush);
                
                if(newInstance != null)
                    _reusePool.Add(args.ReuseId, new RoundedCornerBrushInstance(newInstance, rect));

                return newInstance;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static Geometry GenerateRoundedCornerGeometry(Rect rect, CornerRadius corner)
        {
            // Credit: http://wpftutorial.net/DrawRoundedRectangle.html
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                if (ctx == null)
                    throw new ArgumentNullException(nameof(ctx), "Unable to open the geometry drawing stream context.");

                void ArcTo90DegreeClockwise(Point p, Size s)
                {
                    ctx.ArcTo(p, s, 90, false, SweepDirection.Clockwise);
                }

                ctx.BeginFigure(rect.TopLeft + new Vector(0, corner.TopLeft), true);
                ArcTo90DegreeClockwise(new Point(rect.TopLeft.X + corner.TopLeft, rect.TopLeft.Y),
                    new Size(corner.TopLeft, corner.TopLeft));
                
                ctx.LineTo(rect.TopRight - new Vector(corner.TopRight, 0));
                ArcTo90DegreeClockwise(new Point(rect.TopRight.X, rect.TopRight.Y + corner.TopRight),
                    new Size(corner.TopRight, corner.TopRight));
                
                ctx.LineTo(rect.BottomRight - new Vector(0, corner.BottomRight));
                ArcTo90DegreeClockwise(new Point(rect.BottomRight.X - corner.BottomRight, rect.BottomRight.Y),
                    new Size(corner.BottomRight, corner.BottomRight));
                
                ctx.LineTo(rect.BottomLeft + new Vector(corner.BottomLeft, 0));
                ArcTo90DegreeClockwise(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - corner.BottomLeft),
                    new Size(corner.BottomLeft, corner.BottomLeft));
                
                ctx.EndFigure(true);
            }

            var result = geometry.Clone();

            return result;
        }
        
        private static IBitmap? InitiateBitmapInstance(Geometry? geometry, IBrush brush)
        {
            if (geometry == null)
                return null;
            
            var pen = new Pen(Brushes.Black, 0);
            var rect = geometry.GetRenderBounds(pen);
            var size = new PixelSize((int) Math.Round(rect.Width), (int) Math.Round(rect.Height));

            var visual = new RenderTargetBitmap(size);
            
            using (var ctxBase = visual.CreateDrawingContext(null))
            {
                using (var ctx = new DrawingContext(ctxBase, false))
                {
                    ctxBase.Clear(default);
                    ctx.DrawGeometry(brush, pen, geometry);
                }
            }

            return visual;
        }

        private static IBitmap ReDrawBitmapInstance(IBitmap inst, Geometry? geometry, IBrush brush)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));
            
            var pen = new Pen(Brushes.Black, 0);
            var rect = geometry.GetRenderBounds(pen);
            var size = new PixelSize((int) Math.Round(rect.Width), (int) Math.Round(rect.Height));

            if (inst is not RenderTargetBitmap visual)
                throw new NotSupportedException($"The bitmap instance type {inst.GetType()} is not supported for redraw.");
            
            using (var ctxBase = visual.CreateDrawingContext(null))
            {
                using (var ctx = new DrawingContext(ctxBase, false))
                {
                    ctxBase.Clear(default);
                    ctx.DrawGeometry(brush, pen, geometry);
                }
            }
                
            return visual;
        }
    }
}