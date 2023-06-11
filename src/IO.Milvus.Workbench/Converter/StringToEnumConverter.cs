using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IO.Milvus.Workbench.Converter;

[ValueConversion(typeof(Enum), typeof(string))]
public class EnumToStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString();        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = Enum.Parse(targetType, value?.ToString() ?? string.Empty);
        return result;
    }
}