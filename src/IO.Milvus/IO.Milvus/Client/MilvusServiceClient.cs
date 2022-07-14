
using Grpc.Core;
using Grpc.Net.Client;
using IO.Milvus.Grpc;
using IO.Milvus.Param;
using System;

namespace IO.Milvus.Client
{
    public class MilvusServiceClient:AbstractMilvusGrpcClient
    {
        public MilvusServiceClient(ConnectParam connectParam)
        {
            connectParam.Check();

            defaultCallOptions = new CallOptions();
            defaultCallOptions.WithHeaders(new Metadata()
            {
                {"authorization",connectParam.Authorization }
            });

            channel = GrpcChannel.ForAddress(connectParam.GetAddress());
            
            client = new MilvusService.MilvusServiceClient(channel);
        }
    }
}
