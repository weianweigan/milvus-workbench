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

        R<bool> HasCollection(HasCollectionParam hasCollectionParam);

        /// <summary>
        /// Conducts ANN search on a vector field. Use expression to do filtering before search.
        /// </summary>
        /// <param name="requestParam"><see cref="SearchParam{TVector}"/></param>
        /// <returns></returns>
        R<SearchResults> Search<TVector>(SearchParam<TVector> requestParam);

        /// <summary>
        ///  Creates a collection in Milvus.
        /// </summary>
        /// <param name="requestParam"><see cref="CreateCollectionParam"/></param>
        /// <returns>{status:result code, data:RpcStatus{msg: result message}}</returns>
        R<RpcStatus> CreateCollection(CreateCollectionParam requestParam);

        R<RpcStatus> DropCollection(DropCollectionParam requestParam);

        R<RpcStatus> LoadCollection(LoadCollectionParam requestParam);

        R<RpcStatus> ReleaseCollection(ReleaseCollectionParam requestParam);

        R<DescribeCollectionResponse> DescribeCollection(DescribeCollectionParam requestParam);

        R<GetCollectionStatisticsResponse> GetCollectionStatistics(GetCollectionStatisticsParam requestParam);

        R<ShowCollectionsResponse> ShowCollections(ShowCollectionsParam requestParam);

        R<FlushResponse> Flush(FlushParam requestParam);

        R<RpcStatus> CreatePartition(CreatePartitionParam requestParam);

        R<RpcStatus> DropPartition(DropPartitionParam requestParam);

        R<bool> HasPartition(HasPartitionParam requestParam);

        R<RpcStatus> LoadPartitions(LoadPartitionsParam requestParam);

        R<RpcStatus> ReleasePartitions(ReleasePartitionsParam requestParam);

        R<GetPartitionStatisticsResponse> GetPartitionStatistics(GetPartitionStatisticsParam requestParam);

        R<ShowPartitionsResponse> ShowPartitions(ShowPartitionsParam requestParam);

        R<RpcStatus> CreateAlias(CreateAliasParam requestParam);

        R<RpcStatus> DropAlias(DropAliasParam requestParam);

        R<RpcStatus> AlterAlias(AlterAliasParam requestParam);

        R<RpcStatus> CreateIndex(CreateIndexParam requestParam);

        R<RpcStatus> DropIndex(DropIndexParam requestParam);

        R<DescribeIndexResponse> DescribeIndex(DescribeIndexParam requestParam);

        R<GetIndexBuildProgressResponse> GetIndexBuildProgress(GetIndexBuildProgressParam requestParam);

        R<MutationResult> Insert(InsertParam requestParam);

        Task<R<MutationResult>> InsertAsync(InsertParam requestParam);

        R<MutationResult> Delete(DeleteParam requestParam);

        Task<R<SearchResults>> SearchAsync<TVector>(SearchParam<TVector> requestParam);

        R<QueryResults> Query(QueryParam requestParam);

        Task<R<QueryResults>> QueryAsync(QueryParam requestParam);

        R<CalcDistanceResults> CalcDistance(CalcDistanceParam requestParam);

        R<GetMetricsResponse> GetMetrics(GetMetricsParam requestParam);

        R<GetFlushStateResponse> GetFlushState(GetFlushStateParam requestParam);

        R<GetPersistentSegmentInfoResponse> GetPersistentSegmentInfo(GetPersistentSegmentInfoParam requestParam);

        R<GetQuerySegmentInfoResponse> GetQuerySegmentInfo(GetQuerySegmentInfoParam requestParam);

        //R<GetReplicasResponse> getReplicas(GetReplicasParam requestParam);

        R<RpcStatus> LoadBalance(LoadBalanceParam requestParam);

        R<GetCompactionStateResponse> GetCompactionState(GetCompactionStateParam requestParam);

        R<ManualCompactionResponse> ManualCompaction(ManualCompactionParam requestParam);

        R<GetCompactionPlansResponse> GetCompactionStateWithPlans(GetCompactionPlansParam requestParam);

        R<RpcStatus> CreateCredential(CreateCredentialParam requestParam);

        R<RpcStatus> UpdateCredential(UpdateCredentialParam requestParam);

        R<RpcStatus> DeleteCredential(DeleteCredentialParam requestParam);

        //R<ListCredUsersResponse> listCredUsers(ListCredUsersParam requestParam);


    }
}
