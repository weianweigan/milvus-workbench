using IO.Milvus.Workbench.Models;
using System.Windows;
using System.Windows.Controls;

namespace IO.Milvus.Workbench.Themes;

internal class MilvusParameterTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is MilvusFieldTypeModel fieldTypeModel)
        {
            if (fieldTypeModel.DataType == MilvusDataType.VarChar)
            {
                return MaxLengthDataTemplate;
            }
            else if (fieldTypeModel.IsPrimaryKey)
            {
                return AutoIdDataTemplate;
            }
            else if(fieldTypeModel.DataType == MilvusDataType.FloatVector || fieldTypeModel.DataType == MilvusDataType.BinaryVector)
            {
                return DimensionDataTemplate;
            }
            else
            {
                return NoneDataTemplate;
            }
        }

        return null;
    }

    public DataTemplate AutoIdDataTemplate { get; set; }

    public DataTemplate DimensionDataTemplate { get; set; }

    public DataTemplate MaxLengthDataTemplate { get; set; }

    public DataTemplate NoneDataTemplate { get; set; }
}
