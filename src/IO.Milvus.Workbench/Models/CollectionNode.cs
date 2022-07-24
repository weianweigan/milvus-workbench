using IO.Milvus.Grpc;
using IO.Milvus.Param.Collection;
using IO.Milvus.Param.Partition;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace IO.Milvus.Workbench.Models
{
    public class CollectionNode : Node<PartitionNode>
    {
        private List<FieldModel> _fields;

        public CollectionNode(
                    MilvusConnectionNode parent,
                    string name,
                    long id,
                    ulong createdTimestampsmeStamp,
                    ulong createdUtcTimeStamp)
        {
            Name = name;
            Parent = parent;
            Id = id;
        }

        public MilvusConnectionNode Parent { get; }

        public long Id { get; set; }

        public bool AutoID { get; set; }

        public int ShardsNum { get; set; }

        public List<FieldModel> Fields { get => _fields; set => SetProperty(ref _fields , value); }

        public async Task ConnectAsync()
        {
            var r = Parent.ServiceClient.DescribeCollection(DescribeCollectionParam.Create(Name));

            if (r.Status != Param.Status.Success)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    State = NodeState.Error;
                });
                return;
            }

            AutoID = r.Data.Schema.AutoID;
            Description = r.Data.Schema.Description;
            ShardsNum = r.Data.ShardsNum;

            //Query Partition
            var allPartitionR = Parent.ServiceClient.ShowPartitions(ShowPartitionsParam.Create(Name, null));

            if (allPartitionR.Status != Param.Status.Success)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    State = NodeState.Error;
                });
                return;
            }

            for (int i = 0; i < allPartitionR.Data.PartitionNames.Count; i++)
            {
                var partition = new PartitionNode(
                    allPartitionR.Data.PartitionNames[i],
                    allPartitionR.Data.PartitionIDs[i]);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Children.Add(partition);
                });
            }

            var fields = new List<FieldModel>();
            foreach (var field in r.Data.Schema.Fields)
            {
                var fieldNode = new FieldModel(
                    field.IsPrimaryKey,
                    field.Name,
                    field.FieldID,
                    field.DataType,
                    field.Description);

                if (field.DataType == DataType.FloatVector)
                {
                    var dim = field.TypeParams.FirstOrDefault(p => p.Key == "dim")?.Value;
                    if (dim != null)
                    {
                        fieldNode.Dimension = int.Parse(dim);
                    }
                }

                fields.Add(fieldNode);
            }
            Fields = fields;
        }

        public override string ToString()
        {
            return $"ID:{Id}\nAutoID:{AutoID}\nDescription:{Description}";
        }

    }
}
