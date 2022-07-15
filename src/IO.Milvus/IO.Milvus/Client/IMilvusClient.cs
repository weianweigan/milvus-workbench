using IO.Milvus.Grpc;
using IO.Milvus.Param;
using IO.Milvus.Param.Alias;
using IO.Milvus.Param.Collection;
using IO.Milvus.Param.Control;
using IO.Milvus.Param.Credential;
using IO.Milvus.Param.Dml;
using IO.Milvus.Param.Index;
using IO.Milvus.Param.Partition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IO.Milvus.Client
{
    public interface IMilvusClient
    {
        /// <summary>
        /// Time setting for rpc call
        /// </summary>
        /// <param name="timeSpan">time span</param>
        /// <returns></returns>
        IMilvusClient WithTimeout(TimeSpan timeSpan);

        /// <summary>
        /// Disconnects from a Milvus server with configurable timeout.
        /// </summary>
        void Close();

        #region Collection
        /// <summary>
        /// Checks if a collection exists.
        /// </summary>
        /// <param name="hasCollectionParam"><see cref="HasCollectionParam"/></param>
        /// <returns>{status:result code, data: boolean, whether if has collection or not}</returns>
        R<bool> HasCollection(HasCollectionParam hasCollectionParam);

        /// <summary>
        ///  Creates a collection in Milvus.
        /// </summary>
        /// <param name="requestParam"><see cref="CreateCollectionParam"/></param>
        /// <returns>{status:result code, data:RpcStatus{msg: result message}}</returns>
        R<RpcStatus> CreateCollection(CreateCollectionParam requestParam);

        /// <summary>
        /// Drops a collection. Note that this method drops all data in the collection.
        /// </summary>
        /// <param name="requestParam"><see cref="DropCollectionParam"/> Use <see cref="DropCollectionParam.Create(string)"/></param>
        /// <returns>{status:result code, data:RpcStatus{msg: result message}}</returns>
        R<RpcStatus> DropCollection(DropCollectionParam requestParam);

        /// <summary>
        /// Loads a collection to memory before search or query.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns>{status:result code, data:RpcStatus{msg: result message}}</returns>
        R<RpcStatus> LoadCollection(LoadCollectionParam requestParam);

        /// <summary>
        /// Releases a collection from memory to reduce memory usage. Note that you 
        /// cannot search while the corresponding collection is released from memory.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> ReleaseCollection(ReleaseCollectionParam requestParam);

        /// <summary>
        /// Shows the details of a collection, e.g. name, schema.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<DescribeCollectionResponse> DescribeCollection(DescribeCollectionParam requestParam);

        /// <summary>
        ///  Shows the statistics information of a collection.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetCollectionStatisticsResponse> GetCollectionStatistics(GetCollectionStatisticsParam requestParam);

        /// <summary>
        /// Lists all collections or gets collection loading status.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<ShowCollectionsResponse> ShowCollections(ShowCollectionsParam requestParam);

        /// <summary>
        /// Checks if a collection exists.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<bool> HasCollection(string collectionName);

        /// <summary>
        ///  Drops a collection. Note that this method drops all data in the collection.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<RpcStatus> DropCollection(string collectionName);

        /// <summary>
        /// Shows the details of a collection, e.g. name, schema.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<DescribeCollectionResponse> DescribeCollection(string collectionName);

        /// <summary>
        /// Releases a collection from memory to reduce memory usage. Note that you 
        /// cannot search while the corresponding collection is released from memory.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<RpcStatus> ReleaseCollection(string collectionName);

        /// <summary>
        /// Loads a collection to memory before search or query.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        Task<R<RpcStatus>> LoadCollectionAsync(string collectionName);

        /// <summary>
        /// Loads a collection to memory before search or query.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<RpcStatus> LoadCollection(string collectionName);
        #endregion

        /// <summary>
        ///  Flushes collections.
        /// Currently we do not support this method on client since compaction is not supported on server.     
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<FlushResponse> Flush(FlushParam requestParam);

        #region Partition
        /// <summary>
        /// Creates a partition in the specified collection.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> CreatePartition(CreatePartitionParam requestParam);

        /// <summary>
        /// Drops a partition. Note that this method drops all data in this partition 
        /// and the _default partition cannot be dropped.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> DropPartition(DropPartitionParam requestParam);

        /// <summary>
        /// Checks if a partition exists in the specified collection.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<bool> HasPartition(HasPartitionParam requestParam);

        /// <summary>
        /// Loads a partition into memory.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> LoadPartitions(LoadPartitionsParam requestParam);

        /// <summary>
        ///  Releases a partition from memory.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> ReleasePartitions(ReleasePartitionsParam requestParam);

        /// <summary>
        /// Shows the statistics information of a partition.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetPartitionStatisticsResponse> GetPartitionStatistics(GetPartitionStatisticsParam requestParam);

        /// <summary>
        /// Shows all partitions in the specified collection.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<ShowPartitionsResponse> ShowPartitions(ShowPartitionsParam requestParam);
        #endregion

        #region Alias
        /// <summary>
        /// Creates an alias for a collection.
        /// Alias can be used in search or query to replace the collection name
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> CreateAlias(CreateAliasParam requestParam);

        /// <summary>
        /// Drops an alias for the specified collection.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> DropAlias(DropAliasParam requestParam);

        /// <summary>
        /// Alters alias from a collection to another.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> AlterAlias(AlterAliasParam requestParam);
        #endregion

        #region Index
        /// <summary>
        ///  Creates an index on a vector field in the specified collection.
        ///  Note that index building is an async progress.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> CreateIndex(CreateIndexParam requestParam);

        /// <summary>
        /// Drops the index on a vector field in the specified collection.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> DropIndex(DropIndexParam requestParam);

        /// <summary>
        /// Shows the information of the specified index. Current release of Milvus 
        /// only supports showing latest built index.    
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<DescribeIndexResponse> DescribeIndex(DescribeIndexParam requestParam);

        /// <summary>
        ///  Shows the index building state(in-progress/finished/failed), and the reason for failure (if any).
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetIndexStateResponse> getIndexState(GetIndexStateParam requestParam);

        /// <summary>
        /// Shows the index building progress, such as how many rows are indexed.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetIndexBuildProgressResponse> GetIndexBuildProgress(GetIndexBuildProgressParam requestParam);
        #endregion

        #region Data
        /// <summary>
        /// Inserts entities into a specified collection . Note that you don't need to
        /// input primary key field if auto_id is enabled.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<MutationResult> Insert(InsertParam requestParam);

        /// <summary>
        /// Inserts entities into a specified collection . Note that you don't need to
        /// input primary key field if auto_id is enabled.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        Task<R<MutationResult>> InsertAsync(InsertParam requestParam);

        /// <summary>
        /// Deletes entity(s) based on primary key(s) filtered by boolean expression. Current release 
        /// of Milvus only supports expression in the format "pk_field in [1, 2, ...]"
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<MutationResult> Delete(DeleteParam requestParam);
        #endregion

        #region Search and Query
        /// <summary>
        /// Conducts ANN search on a vector field. Use expression to do filtering before search.
        /// </summary>
        /// <param name="requestParam"><see cref="SearchParam{TVector}"/></param>
        /// <returns></returns>
        Task<R<SearchResults>> SearchAsync<TVector>(SearchParam<TVector> requestParam);

        /// <summary>
        /// Conducts ANN search on a vector field. Use expression to do filtering before search.
        /// </summary>
        /// <param name="requestParam"><see cref="SearchParam{TVector}"/></param>
        /// <returns></returns>
        R<SearchResults> Search<TVector>(SearchParam<TVector> requestParam);

        /// <summary>
        /// Queries entity(s) based on scalar field(s) filtered by boolean expression. 
        /// Note that the order of the returned entities cannot be guaranteed.     
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<QueryResults> Query(QueryParam requestParam);

        /// <summary>
        /// Queries entity(s) based on scalar field(s) filtered by boolean expression. 
        /// Note that the order of the returned entities cannot be guaranteed.     
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        Task<R<QueryResults>> QueryAsync(QueryParam requestParam);
        #endregion

        #region Credential
        //R<RpcStatus> CreateCredential(CreateCredentialParam requestParam);

        //R<RpcStatus> UpdateCredential(UpdateCredentialParam requestParam);

        //R<RpcStatus> DeleteCredential(DeleteCredentialParam requestParam);

        //R<ListCredUsersResponse> listCredUsers(ListCredUsersParam requestParam);
        #endregion

        #region Others
        /// <summary>
        ///  Calculates the distance between the specified vectors.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<CalcDistanceResults> CalcDistance(CalcDistanceParam requestParam);

        /// <summary>
        /// Gets the runtime metrics information of Milvus, returns the result in .json format.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetMetricsResponse> GetMetrics(GetMetricsParam requestParam);

        /// <summary>
        /// Get flush state of specified segments.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetFlushStateResponse> GetFlushState(GetFlushStateParam requestParam);

        /// <summary>
        ///  Gets the information of persistent segments from data node, including row count,
        /// persistence state(growing or flushed), etc.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetPersistentSegmentInfoResponse> GetPersistentSegmentInfo(GetPersistentSegmentInfoParam requestParam);

        /// <summary>
        ///  Gets the query information of segments in a collection from query node, including row count,
        /// memory usage size, index name, etc.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetQuerySegmentInfoResponse> GetQuerySegmentInfo(GetQuerySegmentInfoParam requestParam);

        //R<GetReplicasResponse> getReplicas(GetReplicasParam requestParam);
        /// <summary>
        /// Moves segment from a query node to another to keep the load balanced.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<RpcStatus> LoadBalance(LoadBalanceParam requestParam);

        /// <summary>
        /// Gets the compaction state by id.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetCompactionStateResponse> GetCompactionState(GetCompactionStateParam requestParam);

        /// <summary>
        /// Performs a manual compaction.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<ManualCompactionResponse> ManualCompaction(ManualCompactionParam requestParam);

        /// <summary>
        /// Gets compaction state with its plan.
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        R<GetCompactionPlansResponse> GetCompactionStateWithPlans(GetCompactionPlansParam requestParam);

        /// <summary>
        /// Performs a manual compaction.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<ManualCompactionResponse> ManualCompaction(string collectionName);

        /// <summary>
        ///  Gets the query information of segments in a collection from query node, including row count,
        /// memory usage size, index name, etc.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<GetQuerySegmentInfoResponse> GetQuerySegmentInfo(string collectionName);

        /// <summary>
        /// Gets the information of persistent segments from data node, including row count,
        /// persistence state(growing or flushed), etc.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        R<GetPersistentSegmentInfoResponse> GetPersistentSegmentInfo(string collectionName);

        /// <summary>
        ///  Gets compaction state with its plan.
        /// </summary>
        /// <param name="compactionID"></param>
        /// <returns></returns>
        R<GetCompactionPlansResponse> GetCompactionStateWithPlans(long compactionID);

        /// <summary>
        /// Gets the compaction state by id.
        /// </summary>
        /// <param name="compactionID"></param>
        /// <returns></returns>
        R<GetCompactionStateResponse> GetCompactionState(long compactionID);
        #endregion

    }
}
