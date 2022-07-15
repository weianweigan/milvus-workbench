using IO.Milvus.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.MilvusTests
{
    public class HostConfig
    {
        public const string Host = "192.168.100.139";

        public const int Port = 19530;

        public const string DefaultTestCollectionName = "test";

        public const string DefaultTestPartitionName = "testPartitionName";

        public static ConnectParam ConnectParam { get; } 
            = ConnectParam.Create(Host, Port);
    }
}
