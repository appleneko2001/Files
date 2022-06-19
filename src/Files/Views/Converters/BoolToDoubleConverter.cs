using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Files.Views.Converters.Parameters;

namespace Files.Views.Converters
{
    public class BoolToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Convert bool to double.
        /// </summary>
        /// <param name="value">Boolean value from binding.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Custom value, could be used when value is true.</param>
        /// <param name="culture"></param>
        /// <returns>0 / 1 / value from parameter / null if value is null or invalid object.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool b)
                return null;

            if (parameter is BoolToDoubleConverterParameter p)
            {
                return b ? p.ValueOnTrue : p.ValueOnFalse;
            }
            
            if (double.TryParse(parameter?.ToString(), out var d))
            {
                return b ? d : 0.0;
            }

            return b ? 1.0 : 0.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return d > 0.5;
            }

            return false;
        }
    }
}