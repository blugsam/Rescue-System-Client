using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace RescueSystem.Client.Converters;

public class NullToBoolConverter : IValueConverter
{
    public bool IsInverted { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool result = value is not null;
        return IsInverted ? !result : result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Revert convertion isn't supported by NullToBoolConverter.");
    }
}