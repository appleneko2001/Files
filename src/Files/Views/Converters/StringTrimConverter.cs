using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Files.Views.Converters
{
    public class StringTrimConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Keep end of string with ellipsis if it is too long

            if (value is not string str)
                return value;
            
            const int maxLength = 20;

            if (str.Length <= maxLength)
                return str;
            
            var offset = str.Length - maxLength;
            return "..." + str[offset..];
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}