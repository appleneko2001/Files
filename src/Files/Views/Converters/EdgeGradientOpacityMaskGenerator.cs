using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Files.Views.Converters.Parameters;

namespace Files.Views.Converters
{
    public class EdgeGradientOpacityMaskGenerator : IMultiValueConverter
    {
        // First value should be container bounds
        // second one is scroller horizontal length
        // and third one is scroller horizontal value
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if(parameter is null || values.Count < 3)
                return Array.Empty<GradientStop>();

            // Dp
            double leftGradientDistance, rightGradientDistance;
            
            if (parameter is not EdgeGradientGeneratorParameter p)
            {
                if (parameter is not double d)
                    return Array.Empty<GradientStop>();

                leftGradientDistance = d;
                rightGradientDistance = d;
            }
            else
            {
                leftGradientDistance = p.LeftOpacityDistanceDp;
                rightGradientDistance = p.RightOpacityDistanceDp;
            }

            //    values[1] is not double length || 
            //    values[2] is not double value)
            if (values[0] is not Rect rect)
                return Array.Empty<GradientStop>();

            double length = 0, value = 0;
            
            switch (values[1])
            {
                case double v:
                    length = v;
                    break;
                case Size size:
                    length = size.Width;
                    break;
            }

            switch (values[2])
            {
                case double v:
                    value = v;
                    break;
                case Vector vector:
                    value = vector.X;
                    break;
            }

            if(rect.Width < leftGradientDistance + rightGradientDistance)
                return Array.Empty<GradientStop>();

            var w = rect.Width;

            var l = (byte)(255 - Math.Clamp(Math.Clamp(value, 0, length) / leftGradientDistance, 0, 1) * 255);
            var r = (byte)(255 - Math.Clamp((length - Math.Clamp(value, 0, length)) / rightGradientDistance, 0, 1) * 255);
            
            var leftStartGradient = new GradientStop(Color.FromArgb(l,l,l,l), 0.0);
            var leftEndGradient = new GradientStop(Colors.White, leftGradientDistance / w);
            var rightStartGradient = new GradientStop(Colors.White, (w - rightGradientDistance) / w);
            var rightEndGradient = new GradientStop(Color.FromArgb(r,r,r,r), 1.0);
            var brush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative)
            };
            brush.GradientStops = new GradientStops
            {
                leftStartGradient,
                leftEndGradient,
                rightStartGradient,
                rightEndGradient
            };
            return brush;
        }
    }
}