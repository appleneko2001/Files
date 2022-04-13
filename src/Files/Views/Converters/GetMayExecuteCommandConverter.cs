using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using MinimalMvvm.ViewModels.Commands;
using MinimalMvvm.ViewModels.Commands.Interfaces;

// ReSharper disable HeapView.BoxingAllocation

namespace Files.Views.Converters
{
    /// <summary>
    /// <p>Use this with command based on <see cref="ExtendedRelayCommand"/>.</p>
    /// <p>This converter will return a boolean value from callback <see cref="ExtendedRelayCommand.MayExecute"/>, or return true when value is not IMayExecuteCommand.</p>
    public class GetMayExecuteCommandConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                return true;
            
            if (values[0] is IMayExecuteCommand c)
            {
                return c.MayExecute(values[1]);
            }
            else if (values[1] is IMayExecuteCommand c1)
            {
                return c1.MayExecute(values[0]);
            }

            return true;
        }
        
        /*
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {

            // ReSharper disable once HeapView.BoxingAllocation
            return value is not IMayExecuteCommand command || command.MayExecute(parameter);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }*/
    }
}