using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Files.Views.Converters.Parameters;

namespace Files.Views.Converters
{
    // credit: https://stackoverflow.com/questions/36702865/how-to-shrink-tabitem-size-when-more-tabs-are-added-like-google-chrome-tabs
    public class CountToWidthConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is not CountToWidthConverterParameter p)
                return double.NaN;
            
            if(values[0] is not double containerWidth)
                return double.NaN;
            
            if(values[1] is not int count)
                return double.NaN;
            
            if(count == 0)
                return double.NaN;

            if (p.MarginOffset != null)
            {
                containerWidth = containerWidth - p.MarginOffset?.Right ?? 0;
            }

            var preferredValue = p.MaxWidth;
            var compactSize = containerWidth / count - 16.0;
            
            return preferredValue > compactSize ? Math.Clamp(compactSize, p.MinWidth, p.MaxWidth) : preferredValue;
        }
    }
}