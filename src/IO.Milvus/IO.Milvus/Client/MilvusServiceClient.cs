﻿
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
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            metadata = new Metadata()
            {
                {"authorization",connectParam.Authorization }
            };            

            channel = GrpcChannel.ForAddress($"http://{connectParam.Host}:{connectParam.Port}");
            
            client = new MilvusService.MilvusServiceClient(channel);
        }
    }
}