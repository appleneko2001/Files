using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Files.Extensions;

namespace Files.Views.Converters
{
    public class HumanizeSizeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is long l)
            {
                return l.HumanizeSizeString(culture);
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}