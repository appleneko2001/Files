using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Files.Views.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return parameter is not string format ? value : string.Format(format, value);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}