using System;
using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Windows.Data;

namespace IO.Milvus.Workbench.Converter;

public class ListItemToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IList list)
        {
            return JsonSerializer.Serialize(list);
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
