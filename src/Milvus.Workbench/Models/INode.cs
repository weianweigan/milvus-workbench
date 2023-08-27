using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Milvus.Workbench.Models;

public interface INode
{
    string Name { get; set; }

    bool IsSelected { get; set; }

    bool IsExpanded { get; set; }
}


public abstract class Node : INode
{
    public Node(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public bool IsSelected { get; set; }

    public bool IsExpanded { get; set; }

    public ObservableCollection<INode> Nodes { get; private set; } = new();

    public virtual Task LoadAsync() => Task.CompletedTask;
}