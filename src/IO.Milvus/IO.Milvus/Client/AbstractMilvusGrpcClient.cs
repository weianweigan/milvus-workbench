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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IO.Milvus.Client
{
    public abstract class AbstractMilvusGrpcClient : IMilvusClient
    {
        #region Fields
        protected GrpcChannel channel;
        protected MilvusService.MilvusServiceClient client;
        protected CallOptions defaultCallOptions;
        private TimeSpan? defaultTimeOut;
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

        public IMilvusClient WithTimeout(TimeSpan timeSpan)
        {
            defaultTimeOut = timeSpan;

            return this;
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

        private CallOptions WithInternalOptions()
        {
            if (defaultTimeOut != null)
            {
                var options = defaultCallOptions;
                options.WithDeadline(DateTime.UtcNow.AddSeconds(defaultTimeOut.Value.TotalSeconds));
            }
            return defaultCallOptions;
        }

        private R<T> FailedStatus<T>(string requestName, IO.Milvus.Grpc.Status status)
        {
            var reason = status.Reason;
            if (string.IsNullOrEmpty(reason))
            {
                reason = $"error code: {status.ErrorCode}";
            }
            //logError(requestName + " failed:\n{}", reason);
            return R<T>.Failed(status.ErrorCode, reason);
        }
        #endregion

        #region Api Methods

        #region Collection
        public R<ShowCollectionsResponse> ShowCollections([NotNull] ShowCollectionsParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<ShowCollectionsResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new ShowCollectionsRequest()
                {
                    Type = requestParam.ShowType
                };
                requestParam.CollectionNames.AddRange(requestParam.CollectionNames);

                var response = client.ShowCollections(request,WithInternalOptions());

                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<ShowCollectionsResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<ShowCollectionsResponse>(nameof(ShowCollectionsRequest),response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<ShowCollectionsResponse>.Failed(e);
            }
        }

        public R<RpcStatus> CreateCollection([NotNull] CreateCollectionParam requestParam)
        {
            requestParam.Check();
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
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

                var response = client.CreateCollection(request,WithInternalOptions());

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
                return R<RpcStatus>.Failed(e);
            }
        }

        ///<inheritdoc/>
        public R<RpcStatus> DropCollection([NotNull]DropCollectionParam requestParam)
        {
            return DropCollection(requestParam.CollectionName);
        }

        ///<inheritdoc/>
        public R<RpcStatus> DropCollection([NotNull]string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));

                var request = new DropCollectionRequest()
                {
                    CollectionName = collectionName,
                };

                var response = client.DropCollection(request,WithInternalOptions());

                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(DropCollectionRequest), response);
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }

        public R<bool> HasCollection([NotNull]HasCollectionParam hasCollectionParam)
        {
            return HasCollection(hasCollectionParam.CollectionName);
        }

        public R<bool> HasCollection([NotNull]string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<bool>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));

                var hasCollectionRequest = new HasCollectionRequest()
                {
                    CollectionName = collectionName,
                };

                var response = client.HasCollection(hasCollectionRequest, WithInternalOptions());

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
                return R<bool>.Failed(ex);
            }
        }

        public R<DescribeCollectionResponse> DescribeCollection(DescribeCollectionParam requestParam)
        {
            return DescribeCollection(requestParam.CollectionName);
        }

        public R<DescribeCollectionResponse> DescribeCollection(string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<DescribeCollectionResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));
                var request = new DescribeCollectionRequest()
                {
                    CollectionName = collectionName
                };
                var response = client.DescribeCollection(request, WithInternalOptions());

                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<DescribeCollectionResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<DescribeCollectionResponse>(
                        nameof(DescribeCollectionRequest),
                        response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<DescribeCollectionResponse>.Failed(e);
            }
        }

        public R<GetCollectionStatisticsResponse> GetCollectionStatistics(
            GetCollectionStatisticsParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<GetCollectionStatisticsResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                if (requestParam.IsFlushCollection)
                {
                    var flushResponse = Flush(FlushParam.Create(requestParam.CollectionName));
                    if (flushResponse.Status != Param.Status.Success)
                    {
                        return R<GetCollectionStatisticsResponse>.Failed((ErrorCode)flushResponse.Status, flushResponse.Exception?.Message);
                    }
                }

                var request = new GetCollectionStatisticsRequest()
                {
                    CollectionName = requestParam.CollectionName,
                };

                var response = client.GetCollectionStatistics(request);
                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<GetCollectionStatisticsResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<GetCollectionStatisticsResponse>(nameof(GetCollectionStatisticsRequest), response.Status);
                }
            }
            catch (System.Exception e)
            {
               return R<GetCollectionStatisticsResponse>.Failed(e);
            }
        }

        public R<RpcStatus> LoadCollection(LoadCollectionParam requestParam)
        {
            return LoadCollection(requestParam.CollectionName);
        }

        public R<RpcStatus> LoadCollection(string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {

                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));
                var request = new LoadCollectionRequest()
                {
                    CollectionName = collectionName
                };

                var response = client.LoadCollection(request,WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest), new Grpc.Status { ErrorCode = response.ErrorCode });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }

        public async Task<R<RpcStatus>> LoadCollectionAsync(LoadCollectionParam requestParam)
        {
            return await LoadCollectionAsync(requestParam.CollectionName);
        }

        public async Task<R<RpcStatus>> LoadCollectionAsync(string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));
                var request = new LoadCollectionRequest()
                {
                    CollectionName = collectionName
                };

                var response = await client.LoadCollectionAsync(request,WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest), new Grpc.Status { ErrorCode = response.ErrorCode });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }

        public R<RpcStatus> ReleaseCollection(ReleaseCollectionParam requestParam)
        {
            return ReleaseCollection(requestParam.CollectionName);
        }

        public R<RpcStatus> ReleaseCollection(string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));
                var request = new ReleaseCollectionRequest()
                {
                    CollectionName = collectionName
                };

                var response = client.ReleaseCollection(request,WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest), new Grpc.Status { ErrorCode = response.ErrorCode });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }

        public async Task<R<RpcStatus>> ReleaseCollectionAsync(ReleaseCollectionParam requestParam)
        {
            return await ReleaseCollectionAsync(requestParam.CollectionName);
        }

        public async Task<R<RpcStatus>> ReleaseCollectionAsync(string collectionName)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                ParamUtils.CheckNullEmptyString(collectionName, nameof(collectionName));
                var request = new ReleaseCollectionRequest()
                {
                    CollectionName = collectionName
                };

                var response = await client.ReleaseCollectionAsync(request,WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest), new Grpc.Status { ErrorCode = response.ErrorCode });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }
        #endregion

        #region Partition
        public R<RpcStatus> CreatePartition(CreatePartitionParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new CreatePartitionRequest()
                {
                    CollectionName = requestParam.CollectionName,
                    PartitionName = requestParam.PartitionName,
                };

                var response = client.CreatePartition(request,WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest),
                        new Grpc.Status { ErrorCode = response.ErrorCode });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }

        public R<RpcStatus> DropPartition(DropPartitionParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new DropPartitionRequest()
                {
                    CollectionName = requestParam.CollectionName,
                    PartitionName = requestParam.PartitionName,
                };

                var response = client.DropPartition(request, WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest),
                        new Grpc.Status { ErrorCode = response.ErrorCode });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }

        public R<bool> HasPartition(HasPartitionParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<bool>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new HasPartitionRequest()
                {
                    CollectionName = requestParam.CollectionName,
                    PartitionName = requestParam.PartitionName,
                };

                var response = client.HasPartition(request, WithInternalOptions());
                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<bool>.Sucess(response.Value);
                }
                else
                {
                    return FailedStatus<bool>(nameof(LoadCollectionRequest),response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<bool>.Failed(e);
            }
        }

        public R<RpcStatus> LoadPartitions(LoadPartitionsParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new LoadPartitionsRequest()
                {
                    CollectionName = requestParam.CollectionName,
                };
                request.PartitionNames.AddRange(requestParam.PartitionNames);

                var response = client.LoadPartitions(request, WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest),new Grpc.Status() 
                    { ErrorCode = response.ErrorCode,
                        Reason = response.Reason});
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }
        
        public R<RpcStatus> ReleasePartitions(ReleasePartitionsParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<RpcStatus>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new ReleasePartitionsRequest()
                {
                    CollectionName = requestParam.CollectionName,
                };
                request.PartitionNames.AddRange(requestParam.PartitionNames);

                var response = client.ReleasePartitions(request, WithInternalOptions());
                if (response.ErrorCode == ErrorCode.Success)
                {
                    return R<RpcStatus>.Sucess(new RpcStatus(RpcStatus.SUCCESS_MSG));
                }
                else
                {
                    return FailedStatus<RpcStatus>(nameof(LoadCollectionRequest), new Grpc.Status()
                    {
                        ErrorCode = response.ErrorCode,
                        Reason = response.Reason
                    });
                }
            }
            catch (System.Exception e)
            {
                return R<RpcStatus>.Failed(e);
            }
        }
        
        public R<GetPartitionStatisticsResponse> GetPartitionStatistics(GetPartitionStatisticsParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<GetPartitionStatisticsResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();

                if (requestParam.IsFulshCollection)
                {
                    var flushResponse = Flush(FlushParam.Create(requestParam.CollectionName));
                    if (flushResponse.Status != Param.Status.Success)
                    {
                        return R<GetPartitionStatisticsResponse>.Failed((ErrorCode)flushResponse.Status,flushResponse.Exception);
                    }
                }
                
                var request = new GetPartitionStatisticsRequest()
                {
                    CollectionName = requestParam.CollectionName,
                    PartitionName = requestParam.PartitionName
                };

                var response = client.GetPartitionStatistics(request, WithInternalOptions());
                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<GetPartitionStatisticsResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<GetPartitionStatisticsResponse>(nameof(GetPartitionStatisticsRequest),response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<GetPartitionStatisticsResponse>.Failed(e);
            }
        }

        public R<ShowPartitionsResponse> ShowPartitions(ShowPartitionsParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<ShowPartitionsResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();
                var request = new ShowPartitionsRequest()
                {
                    CollectionName = requestParam.CollectionName,
                    Type = requestParam.ShowType,
                };
                request.PartitionNames.Add(requestParam.PartitionNames);

                var response = client.ShowPartitions(request, WithInternalOptions());

                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<ShowPartitionsResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<ShowPartitionsResponse>(
                        nameof(DescribeCollectionRequest),
                        response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<ShowPartitionsResponse>.Failed(e);
            }
        }
        #endregion

        #region Alias
        public R<RpcStatus> CreateAlias(CreateAliasParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> AlterAlias(AlterAliasParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> DropAlias(DropAliasParam requestParam)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Data
        public R<MutationResult> Insert(InsertParam requestParam)
        {
            throw new NotImplementedException();
        }

        public Task<R<MutationResult>> InsertAsync(InsertParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<MutationResult> Delete(DeleteParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<FlushResponse> Flush(FlushParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<FlushResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();

                var request = new FlushRequest();
                request.CollectionNames.AddRange(requestParam.CollectionNames);

                var response = client.Flush(request);
                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<FlushResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<FlushResponse>(nameof(FlushRequest), response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<FlushResponse>.Failed(e);
            }
        }

        public async Task<R<FlushResponse>> FlushAsync(FlushParam requestParam)
        {
            if (!ClientIsReady())
            {
                return R<FlushResponse>.Failed(new ClientNotConnectedException("Client rpc channel is not ready"));
            }

            try
            {
                requestParam.Check();

                var request = new FlushRequest();
                request.CollectionNames.AddRange(requestParam.CollectionNames);

                var response = await client.FlushAsync(request);
                if (response.Status.ErrorCode == ErrorCode.Success)
                {
                    return R<FlushResponse>.Sucess(response);
                }
                else
                {
                    return FailedStatus<FlushResponse>(nameof(FlushRequest), response.Status);
                }
            }
            catch (System.Exception e)
            {
                return R<FlushResponse>.Failed(e);
            }
        }

        public R<GetFlushStateResponse> GetFlushState(GetFlushStateParam requestParam)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Index
        public R<RpcStatus> CreateIndex(CreateIndexParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> DropIndex(DropIndexParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<DescribeIndexResponse> DescribeIndex(DescribeIndexParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetIndexBuildProgressResponse> GetIndexBuildProgress(GetIndexBuildProgressParam requestParam)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Search and Query
        public R<QueryResults> Query(QueryParam requestParam)
        {
            throw new NotImplementedException();
        }

        public Task<R<QueryResults>> QueryAsync(QueryParam requestParam)
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

        public R<CalcDistanceResults> CalcDistance(CalcDistanceParam requestParam)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Credential
        public R<RpcStatus> CreateCredential(CreateCredentialParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> DeleteCredential(DeleteCredentialParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<RpcStatus> UpdateCredential(UpdateCredentialParam requestParam)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Others
        public R<GetCompactionStateResponse> GetCompactionState(GetCompactionStateParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetCompactionPlansResponse> GetCompactionStateWithPlans(GetCompactionPlansParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<GetMetricsResponse> GetMetrics(GetMetricsParam requestParam)
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

        public R<RpcStatus> LoadBalance(LoadBalanceParam requestParam)
        {
            throw new NotImplementedException();
        }

        public R<ManualCompactionResponse> ManualCompaction(ManualCompactionParam requestParam)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

    }
}
