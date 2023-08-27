using Milvus.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milvus.Workbench.Models;

public sealed class MilvusVectorDbNode : Node
{
    private readonly VectorDb _vectorDb;

    public MilvusVectorDbNode(VectorDb vectorDb) : base(vectorDb.Name!)
    {
        _vectorDb = vectorDb;
    }

    public async override Task LoadAsync()
    {
        (MilvusClient milvusClient, string? databaseName)= _vectorDb switch
        {
            MilvusVectorDb db => (new MilvusClient(db.Host!, db.Port, db.UserName, db.Password, db.DatabaseName), db.DatabaseName),
            ZillizServerlessVectorDb db => (new MilvusClient(db.Host!, db.Port, apiKey: db.Apikey, db.DatabaseName), db.DatabaseName),
            _ => throw new System.NotSupportedException(_vectorDb.Name)
        };

        databaseName ??= "_default";
        var databaseNode = new MilvusDatabaseNode(this, milvusClient, databaseName);

        Nodes.Add(databaseNode);

        await databaseNode.LoadAsync();
    }
}

public sealed class MilvusDatabaseNode : Node
{
    public MilvusDatabaseNode(
        MilvusVectorDbNode parent, 
        MilvusClient milvusClient, 
        string databaseName):base(databaseName)
    {
        Parent = parent;
        MilvusClient = milvusClient;
    }

    public MilvusVectorDbNode Parent { get; }

    public MilvusClient MilvusClient { get; }

    public async override Task LoadAsync()
    {
        IReadOnlyList<MilvusCollectionInfo> collectionInfos = await MilvusClient.ListCollectionsAsync();

        foreach (var collectionInfo in collectionInfos)
        {
            var collectionNode = new MilvusCollectionNode(this, MilvusClient.GetCollection(collectionInfo.Name), collectionInfo);
            Nodes.Add(collectionNode);
            await collectionNode.LoadAsync();
        }
    }
}

public class MilvusCollectionNode : Node
{
    private readonly MilvusCollectionInfo _milvusCollectionInfo;

    public MilvusCollectionNode(
        MilvusDatabaseNode parent,
        MilvusCollection collection, 
        MilvusCollectionInfo milvusCollectionInfo) : 
        base(milvusCollectionInfo.Name)
    {
        Parent = parent;
        Collection = collection;
        _milvusCollectionInfo = milvusCollectionInfo;
    }

    public MilvusDatabaseNode Parent { get; }

    public MilvusCollection Collection { get; }

    public async override Task LoadAsync()
    {
        IList<MilvusPartition> partitions = await Collection.ShowPartitionsAsync();
        foreach (var partition in partitions)
        {
            var partitionNode = new MilvusPartitionNode(this, partition);
            Nodes.Add(partitionNode);
            await partitionNode.LoadAsync();
        }
    }

    public override string ToString() => _milvusCollectionInfo.ToString();
}

public sealed class MilvusPartitionNode : Node
{
    private readonly MilvusPartition _milvusPartition;

    public MilvusPartitionNode(MilvusCollectionNode parent,
        MilvusPartition milvusPartition) : 
        base(milvusPartition.PartitionName)
    {
        Parent = parent;
        _milvusPartition = milvusPartition;
    }

    public MilvusCollectionNode Parent { get; }

    public override string ToString() => _milvusPartition.ToString();
}