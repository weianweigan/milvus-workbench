using System.Collections.Generic;

namespace IO.Milvus.Workbench.Models;

public class IndexNode : Node
{
    public IndexNode(
        CollectionNode parent,
        string indexName, 
        string fieldName, 
        IDictionary<string, string> @params)
    {
        Parent = parent;
        Name = indexName;
        FieldName = fieldName;
        Params = @params;
    }

    public CollectionNode Parent { get; private set; }

    public string FieldName { get; }

    public IDictionary<string, string> Params { get; }

    public override IEnumerable<Node> GetChildren()
    {
        yield break;
    }

    public override string ToString()
    {
        return Name;
    }
}
