using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace RescueSystem.Client.Converters;

public class DateOnlyToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
        {
            return new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue));
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            return DateOnly.FromDateTime(dateTimeOffset.DateTime);
        }
        return null;
    }
}