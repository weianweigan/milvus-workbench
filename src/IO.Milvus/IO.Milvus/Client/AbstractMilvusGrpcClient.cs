using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using IO.Milvus.Exception;
using IO.Milvus.Grpc;
using IO.Milvus.Param;
using IO.Milvus.Param.Alias;
using IO.Milvus.Param.Collection;
using IO.Milvus.Param.Control;
using IO.Milvus.Param.Credential;
using IO.Milvus.Param.Dml;
using IO.Milvus.Param.Index;
using IO.Milvus.Param.Partition;
using IO.Milvus.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IO.Milvus.Client
{
    public abstract class AbstractMilvusGrpcClient : IMilvusClient
    {
        #region Fields
        protected GrpcChannel channel;
        protected MilvusService.MilvusServiceClient client;
        protected Metadata metadata;
        #endregion

        #region Public Methods
        public bool ClientIsReady()
        {
            return channel.State != ConnectivityState.Shutdown;
        }

        public void Close()
        {
            channel.ShutdownAsync().Wait();
            channel.Dispose();
        }

        public async Task CloseAsync()
        {
            await channel.ShutdownAsync();
            channel.Dispose();
        }
        #endregion

        #region Private Methods
        private List<Grpc.KeyValuePair> AssembleKvPair(Dictionary<string,string> sourceDic)
        {
            var result = new List<Grpc.KeyValuePair>();
            if (sourceDic.IsNotEmpty())
            {
                foreach (var kv in sourceDic)
                {
                    result.Add(new Grpc.KeyValuePair()
                    {
                        Key = kv.Key,
                        Value = kv.Value
                    });
                }
            }
            return result;
        }
        #endregion

        #region Api Methods
        public R<RpcStatus> AlterAlias(AlterAliasParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<CalcDistanceResults> CalcDistance(CalcDistanceParam requestParam)
        {
            throw new NotImplementedException();
        }

        private R<T> FailedStatus<T>(string requestName, IO.Milvus.Grpc.Status status)
        {
            var reason = status.Reason;
            if (string.IsNullOrEmpty(reason))
            {
                reason = $"error code: {status.ErrorCode}" ;
            }
            //logError(requestName + " failed:\n{}", reason);
            return R<T>.Failed<T>(status.ErrorCode, reason);
        }

        public R<RpcStatus> CreateAlias(CreateAliasParam requestParam)
        {
            throw new NotImplementedException();
        }

        #region Collection
        public R<ShowCollectionsResponse> ShowCollections(ShowCollectionsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> CreateCollection(CreateCollectionParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed<RpcStatus>(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                var schema = new CollectionSchema()
                {
                    Name = requestParam.CollectionName,
                    Description = requestParam.Description,
                };

                long fieldID = 0;
                foreach (var fieldType in requestParam.FieldTypes)
                {
                    var field = new FieldSchema()
                    {
                        FieldID = fieldID++,
                        Name = fieldType.Name,
                        IsPrimaryKey = fieldType.IsPrimaryKey,
                        Description = fieldType.Description,
                        DataType = fieldType.DataType,
                        AutoID = fieldType.IsAutoID,
                    };

                    var typeParamsList = AssembleKvPair(fieldType.TypeParams);

                    foreach (var item in typeParamsList)
                    {
                        field.TypeParams.Add(item);
                    }

                    schema.Fields.Add(field);
                }
                
                var request = new CreateCollectionRequest()
                {
                    CollectionName = requestParam.CollectionName,
                    ShardsNum = requestParam.ShardsNum,
                    Schema = schema.ToByteString()
                };

                var response = client.CreateCollection(request);

                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(CreateCollectionRequest), response);
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed<RpcStatus>(e);
            }
        }

        public R<RpcStatus> DropCollection(DropCollectionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<bool> HasCollection(HasCollectionParam hasCollectionParam)
        {
            if (!ClientIsReady())
            {
                return R<bool>.Failed<bool>(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                var hasCollectionRequest = new HasCollectionRequest()
                {
                    CollectionName = hasCollectionParam.CollectionName,
                };
                var response = client.HasCollection(hasCollectionRequest, metadata);
                
                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<bool>.Sucess(response.Value);
                }
                else
                {
                    return FailedStatus<bool>(nameof(HasCollectionRequest), response.Status);
                }
            }
            catch (System.Exception ex)
            {
                return R<bool>.Failed<bool>(ex);
            }
        }

        #endregion

        public R<RpcStatus> CreateCredential(CreateCredentialParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> CreateIndex(CreateIndexParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> CreatePartition(CreatePartitionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<MutationResult> Delete(DeleteParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> DeleteCredential(DeleteCredentialParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<DescribeCollectionResponse> DescribeCollection(DescribeCollectionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<DescribeIndexResponse> DescribeIndex(DescribeIndexParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> DropAlias(DropAliasParam requestParam)
        {
            throw new NotImplementedException();
        }


        public R<RpcStatus> DropIndex(DropIndexParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> DropPartition(DropPartitionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<FlushResponse> Flush(FlushParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetCollectionStatisticsResponse> GetCollectionStatistics(GetCollectionStatisticsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetCompactionStateResponse> GetCompactionState(GetCompactionStateParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetCompactionPlansResponse> GetCompactionStateWithPlans(GetCompactionPlansParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetFlushStateResponse> GetFlushState(GetFlushStateParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetIndexBuildProgressResponse> GetIndexBuildProgress(GetIndexBuildProgressParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetMetricsResponse> GetMetrics(GetMetricsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetPartitionStatisticsResponse> GetPartitionStatistics(GetPartitionStatisticsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetPersistentSegmentInfoResponse> GetPersistentSegmentInfo(GetPersistentSegmentInfoParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetQuerySegmentInfoResponse> GetQuerySegmentInfo(GetQuerySegmentInfoParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<bool> HasPartition(HasPartitionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<MutationResult> Insert(InsertParam requestParam)
        {
            throw new NotImplementedException();
        }

        public Task<R<MutationResult>> InsertAsync(InsertParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> LoadBalance(LoadBalanceParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> LoadCollection(LoadCollectionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> LoadPartitions(LoadPartitionsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<ManualCompactionResponse> ManualCompaction(ManualCompactionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<QueryResults> Query(QueryParam requestParam)
        {
            throw new NotImplementedException();
        }

        public Task<R<QueryResults>> QueryAsync(QueryParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> ReleaseCollection(ReleaseCollectionParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> ReleasePartitions(ReleasePartitionsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<SearchResults> Search<TVector>(SearchParam<TVector> requestParam)
        {
            throw new NotImplementedException();
        }

        public Task<R<SearchResults>> SearchAsync<TVector>(SearchParam<TVector> requestParam)
        {
            throw new NotImplementedException();
        }

        public R<ShowPartitionsResponse> ShowPartitions(ShowPartitionsParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> UpdateCredential(UpdateCredentialParam requestParam)
        {
            throw new NotImplementedException();
        }

        public IMilvusClient WithTimeout(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
