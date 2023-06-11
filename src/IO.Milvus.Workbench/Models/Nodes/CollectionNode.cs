using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Dialogs;
using IO.Milvus.Diagnostics;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using System.Collections;
using System.Text.Json;
using IO.Milvus.Grpc;

namespace IO.Milvus.Workbench.Models;

public enum LoadedState
{
    Unknown,
    Loading,
    Loaded,
}

public partial class CollectionNode : Node<Node>
{
    #region Fields
    private List<MilvusFieldTypeModel> _fields;
    private RelayCommand _resetCmd;
    private string _queryText;
    private LoadedState _loadedState;
    private List<string> _alias;
    private AsyncRelayCommand _loadCollectionCmd;
    private AsyncRelayCommand _releaseCollectionCmd;
    private AsyncRelayCommand _dropCollectionCmd;
    private AsyncRelayCommand _createPartitionCmd;
    private AsyncRelayCommand _createIndexCmd;
    private AsyncRelayCommand<ListView> _queryCmd;
    private DataTable _queryResultData;
    #endregion

    #region Ctor
    public CollectionNode(
                DatabaseNode parent,
                string name,
                long id)
    {
        Name = name;
        Parent = parent;
        Id = id;
    }
    #endregion

    #region Properties
    public DatabaseNode Parent { get; }

    public long Id { get; set; }

    public bool AutoID { get; set; }

    public int ShardsNum { get; set; }

    public LoadedState LoadedState { get => _loadedState; set => SetProperty(ref _loadedState, value); }

    public List<MilvusFieldTypeModel> Fields { get => _fields; set => SetProperty(ref _fields, value); }

    public List<string> Aliases { get => _alias; set => SetProperty(ref _alias, value); }

    public List<MilvusIndex> Indexes { get; set; }

    public string QueryText
    {
        get => _queryText; set
        {
            _queryText = value;
            QueryCmd.NotifyCanExecuteChanged();
        }
    }

    public DataTable QueryResultData { get => _queryResultData; set => SetProperty(ref _queryResultData, value); }

    public AsyncRelayCommand CreateIndexCmd { get => _createIndexCmd ?? (_createIndexCmd = new AsyncRelayCommand(CreateIndexClickAsync));}

    public AsyncRelayCommand LoadCollectionCmd { get => _loadCollectionCmd ?? (_loadCollectionCmd = new AsyncRelayCommand(LoadCollectionClickAsync)); }

    public AsyncRelayCommand ReleaseCollectionCmd { get => _releaseCollectionCmd ?? (_releaseCollectionCmd = new AsyncRelayCommand(ReleaseCollectionClickAsync)); }

    public AsyncRelayCommand DropCollectionCmd { get => _dropCollectionCmd ?? (_dropCollectionCmd = new AsyncRelayCommand(DropCollectionClickAsync)); }

    public AsyncRelayCommand CreatePartitionCmd { get => _createPartitionCmd ?? (_createPartitionCmd = new AsyncRelayCommand(CreatePartitionClickAsync));}

    public AsyncRelayCommand<ListView> QueryCmd { get => _queryCmd ?? (_queryCmd = new AsyncRelayCommand<ListView>(QueryClickAsync, (v) => !string.IsNullOrWhiteSpace(QueryText))); }

    public RelayCommand ResetCmd { get => _resetCmd ?? (_resetCmd = new RelayCommand(ResetClick)); }
    #endregion

    #region Private Methods
    private void ResetClick()
    {
        QueryResultData = null;
        ResetCmd.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void OpenSearchPage()
    {
        Parent.Parent.Parent.Search();
    }

    private async Task CreateIndexClickAsync(CancellationToken cancellationToken)
    {
        try
        {
            var dialog = new CreateMilvusIndexDialog(Name,
                Fields.FirstOrDefault(p => p.DataType == MilvusDataType.FloatVector || p.DataType == MilvusDataType.BinaryVector)?.Name);

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var index = dialog.Vm.Indexes.First();

            await Parent.MilvusClient.CreateIndexAsync(
                Name,
                index.FieldName,
                index.IndexName,
                index.IndexType,
                index.MetricType,
                null,
                cancellationToken);

            Indexes = (await Parent.MilvusClient.DescribeIndexAsync(Name, "", cancellationToken)).ToList();

            await ConnectAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task DropCollectionClickAsync()
    {
        var res = MessageBox.Show($"Are you sure delete:{Name}?","Delete",MessageBoxButton.OKCancel,MessageBoxImage.Warning);
        if (res != MessageBoxResult.OK)
        {
            return;
        }
        try
        {
            await Parent.MilvusClient.DropCollectionAsync(Name);
            await Parent.RefreshAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task LoadCollectionClickAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Parent.MilvusClient.LoadCollectionAsync(Name);

            var progressValues = Parent.MilvusClient.WaitForLoadingProgressCollectionValueAsync(
                Name,
                null,
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromSeconds(10),cancellationToken);

            await foreach (var progress in progressValues)
            {
                LoadedState = LoadedState.Loading;
            }

            LoadedState = LoadedState.Loaded;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task CreatePartitionClickAsync(CancellationToken cancellationToken)
    {
        try
        {
            var dialog = new CreatePartitionDialog();
            if (dialog.ShowDialog() != true)
                return;

            await Parent.MilvusClient.CreatePartitionAsync(
                Name,
                dialog.VM.PartitionName,
                cancellationToken);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task ReleaseCollectionClickAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Parent.MilvusClient.ReleaseCollectionAsync(Name, cancellationToken);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task QueryClickAsync(ListView listView,CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(QueryText))
        {
            MessageBox.Show("Please Input QueryText First");
            return;
        }
        try
        {
            var r = await Parent.MilvusClient.QueryAsync(Name, QueryText, Fields.Select(p => p.Name).ToList(),cancellationToken:cancellationToken);

            QueryResultData = ToDataTable(listView, r.FieldsData);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }

    private static DataTable ToDataTable(ListView listView, IList<Field> fieldData)
    {
        if (!(fieldData?.FirstOrDefault()?.RowCount > 0))
        {
            return null;
        }

        //Generate GridView
        var gridView = new GridView();
        DataTable dt = new DataTable();

        for (int i = 0; i < fieldData.Count; i++)
        {
            var field = fieldData[i];

            dt.Columns.Add(field.FieldName, typeof(string));
            var column = new GridViewColumn()
            {
                Header = field.FieldName,
                DisplayMemberBinding = new Binding(field.FieldName),
                Width = listView.ActualWidth / fieldData.Count - 10
            };
            gridView.Columns.Add(column);
        }

        listView.View = gridView;

        var rowCount = fieldData.First().RowCount;

        for (int i = 0; i < rowCount; i++)
        {
            object[] row = new object[fieldData.Count];

            for (int j = 0; j < fieldData.Count; j++)
            {
                var field = fieldData[j];
                if (field is FloatVectorField floatVectorField && floatVectorField.Data.Count > i)
                {
                    row[j] = JsonSerializer.Serialize(floatVectorField.Data[i]);
                }
                else if (field is BinaryVectorField binaryVectorField && binaryVectorField.Data.Count > i)
                {
                    row[j] = JsonSerializer.Serialize(binaryVectorField.Data[i]);
                }
                else
                {
                    var list = field.GetType().GetProperty("Data")?.GetValue(field) as IList;
                    if (list != null && list.Count > i)
                    {
                        row[j] = list[i];
                    }
                }
            }

            dt.Rows.Add(row);
        }

        listView.View = gridView;
        return dt;
    }
    #endregion

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var collectionInfo = await Parent.MilvusClient.DescribeCollectionAsync(Name);
            Id = collectionInfo.CollectionId;
            AutoID = collectionInfo.Schema.AutoId;
            Description = collectionInfo.Schema.Description;
            ShardsNum = collectionInfo.ShardsNum;
            Aliases = collectionInfo.Aliases.ToList();

            Fields = collectionInfo.Schema.Fields.Select(f => new MilvusFieldTypeModel(f)).ToList();

            //Indexes
            var vectorField = Fields.FirstOrDefault(p => p.DataType == MilvusDataType.FloatVector || p.DataType == MilvusDataType.BinaryVector);
            if (vectorField != null)
            {
                try
                {
                    Indexes = (await Parent.MilvusClient.DescribeIndexAsync(Name, vectorField.Name, cancellationToken))?.ToList();
                    if (Indexes?.Any() == true)
                    {
                        foreach (var index in Indexes)
                        {
                            var indexNode = new IndexNode(this, index.IndexName, index.FieldName, index.Params);
                            Children.Add(indexNode);
                        }
                    }
                }
                catch (MilvusException ex)
                {
                    if (ex.ErrorCode != Grpc.ErrorCode.IndexNotExist)
                    {
                        throw;
                    }
                }
            }

            //Partitions
            IList<MilvusPartition> partitions = await Parent.MilvusClient.ShowPartitionsAsync(Name, cancellationToken);

            foreach (var partition in partitions)
            {
                var partitionNode = new PartitionNode(this, partition.PartitionName, partition.PartitionId);
                Children.Add(partitionNode);
            }
        }
        catch (MilvusException e)
        {
            State = NodeState.Error;
            Msg = e.Message;
            return;
        }
        catch(Exception e)
        {
            State = NodeState.Error;
            Msg = e.Message;
            return;
        }
    }

    public override string ToString()
    {
        return $"ID:{Id}\nAutoID:{AutoID}\nDescription:{Description}";
    }
}
