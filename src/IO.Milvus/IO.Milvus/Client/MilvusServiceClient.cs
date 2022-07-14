
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
            metadata = new Metadata()
            {
                {"authorization",connectParam.Authorization }
            };            

            string httpType = connectParam.UseHttps ? "https" : "http";
            channel = GrpcChannel.ForAddress($"{httpType}://{connectParam.Host}:{connectParam.Port}");
            
            client = new MilvusService.MilvusServiceClient(channel);
        }
    }
}
