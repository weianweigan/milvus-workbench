using IO.Milvus.Grpc;
using IO.Milvus.Param.Collection;
using IO.Milvus.Param.Partition;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Param.Dml;

namespace IO.Milvus.Workbench.Models
{
    public class CollectionNode : Node<PartitionNode>
    {
        private List<FieldModel> _fields;
        private AsyncRelayCommand _queryCmd;
        private RelayCommand _resetCmd;
        private string _queryText;
        private List<FieldData> _queryResultData;

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

        #region Properties
        public MilvusConnectionNode Parent { get; }

        public long Id { get; set; }

        public bool AutoID { get; set; }

        public int ShardsNum { get; set; }

        public List<FieldModel> Fields { get => _fields; set => SetProperty(ref _fields, value); }

        public List<FieldData> QueryResultData { get => _queryResultData; set => SetProperty(ref _queryResultData ,value); }

        public string QueryText
        {
            get => _queryText; set
            {
                _queryText = value;
                QueryCmd.NotifyCanExecuteChanged();
            }
        }

        public AsyncRelayCommand QueryCmd { get => _queryCmd ?? (_queryCmd = new AsyncRelayCommand(QueryClickAsync, () => !string.IsNullOrWhiteSpace(QueryText))); }

        public RelayCommand ResetCmd { get => _resetCmd ?? (_resetCmd = new RelayCommand(ResetClick,() => QueryResultData != null)); }

        private void ResetClick()
        {
            QueryResultData = null;
            ResetCmd.NotifyCanExecuteChanged();
        }

        private async Task QueryClickAsync()
        {
            if (string.IsNullOrWhiteSpace(QueryText))
            {
                MessageBox.Show("Please Input QueryText First");
                return;
            }

            var r = await Parent.ServiceClient.QueryAsync(QueryParam.Create(Name,
                Children.Select(p => p.Name).ToList(),
                Fields.Select(p => p.Name).ToList(),
                expr: QueryText));

            if (r.Status != Param.Status.Success)
            {
                MessageBox.Show(r.Exception.Message);
                return;
            }

            QueryResultData = r.Data.FieldsData.ToList();
        }
        #endregion

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
