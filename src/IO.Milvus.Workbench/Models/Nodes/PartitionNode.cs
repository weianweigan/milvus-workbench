using System.Collections.Generic;

namespace IO.Milvus.Workbench.Models
{
    public class PartitionNode : Node
    {
        public PartitionNode(string name, long id)
        {
            Name = name;
            ID = id;
        }

        public long ID { get; }

        public override IEnumerable<Node> GetChildren()
        {
            yield break;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}:{Name}\n{nameof(ID)}:{ID}";
        }
    }
}
