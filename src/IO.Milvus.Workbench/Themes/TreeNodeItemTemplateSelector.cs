using IO.Milvus.Workbench.Models;
using System.Windows;
using System.Windows.Controls;

namespace IO.Milvus.Workbench.Themes;

public class TreeNodeItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate MilvusConnectionNodeDT { get; set; }

    public DataTemplate CollectionNodeDT { get; set; }

    public DataTemplate PartitionNodeDT { get; set; }

    public DataTemplate IndexNodeDT { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item == null)
        {
            return null;
        }

        if (item is DatabaseNode)
        {
            return MilvusConnectionNodeDT;
        }
        else if(item is CollectionNode)
        {
            return CollectionNodeDT;
        }
        else if (item is PartitionNode)
        {
            return PartitionNodeDT;
        }else if (item is IndexNode)
        {
            return IndexNodeDT;
        }

        return null;
    }
}
