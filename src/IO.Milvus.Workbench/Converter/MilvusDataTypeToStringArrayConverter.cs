using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IO.Milvus.Workbench.Converter;

[ValueConversion(typeof(Enum), typeof(Array))]
public class MilvusDataTypeToStringArrayConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is Type type && value is Models.MilvusFieldTypeModel fieldType)
        {
            var values = type.GetEnumNames().Where(p => p != nameof(MilvusIndexType.INVALID));
            if (fieldType.IsPrimaryKey)
            {
                return values.Where(p => p == nameof(MilvusDataType.VarChar) || p == nameof(MilvusDataType.Int64));
            }else if(fieldType.DataType == MilvusDataType.FloatVector || fieldType.DataType == MilvusDataType.BinaryVector)
            {
                return values.Where(p => p == nameof(MilvusDataType.FloatVector) || p == nameof(MilvusDataType.BinaryVector));
            }
            else
            {
                return values.Where(p => p != nameof(MilvusDataType.FloatVector) && p != nameof(MilvusDataType.BinaryVector));
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}