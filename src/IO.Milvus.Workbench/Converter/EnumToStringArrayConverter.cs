using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IO.Milvus.Workbench.Converter;

[ValueConversion(typeof(Enum), typeof(Array))]
public class EnumToStringArrayConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is Type type)
        {
            return type.GetEnumNames().Where(p => p != nameof(MilvusIndexType.INVALID));
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
