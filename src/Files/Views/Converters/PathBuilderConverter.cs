using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Files.Views.Converters.Parameters.PathGen;

namespace Files.Views.Converters
{
    public class PathBuilderConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var geometry = new PathGeometry();
            using (var ctx = geometry.Open())
            {
                if (parameter is IPathGenRecipe recipe &&
                    value is Rect bounds)
                    recipe.BuildRecipe(ctx, bounds);
                
                return geometry;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}