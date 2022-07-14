using IO.Milvus.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.Milvus.Param;
using IO.Milvus.Param.Collection;

namespace IO.Milvus.Client.Tests
{
    [TestClass()]
    public class MilvusServiceClientTests
    {
        [TestMethod()]
        public void MilvusServiceClientTest()
        {
            var service = DefaultService();

            Assert.IsNotNull(service);
            Assert.IsTrue(service.ClientIsReady());
        }

        [TestMethod()]
        [DataRow("test", true)]
        [DataRow("test2", false)]
        public void HasCollectionTest(string collectionName, bool exist)
        {
            var service = DefaultService();

            var r = service.HasCollection(
                HasCollectionParam.NewBuilder()
                .WithCollcetionName(collectionName)
                .Build());

            Assert.IsTrue(r.Status == Status.Success);
            Assert.IsTrue(r.Data == exist);
        }

        public static MilvusServiceClient DefaultService()
        {
            var service = new MilvusServiceClient(
                ConnectParam.NewBuilder()
                .WithHost("192.168.100.139")
                .WithPort(19530)
                .Build());

            Assert.IsNotNull(service);
            Assert.IsTrue(service.ClientIsReady());

            return service;
        }

        [TestMethod()]
        public void CloseTest()
        {
            var service = DefaultService();
            service.Close();
        }

        [TestMethod()]
        public async Task CloseAsyncTest()
        {
            var service = DefaultService();
            await service.CloseAsync();
        }

        [TestMethod()]
        public void CreateCollectionTest()
        {
            var rd = new Random(DateTime.Now.Second);
            var collectionName = $"test{rd.Next()}";
            var service = DefaultService();
            var r = service.CreateCollection(CreateCollectionParam
                .NewBuilder()
                .WithCollectionName(collectionName)
                .WithFieldTypes(new List<FieldType>()                
                {
                    FieldType.NewBuilder()
                    .WithName($"priKey{rd.Next()}")
                    .WithDataType(Grpc.DataType.Int64)
                    .WithIsPrimaryKey(true)
                    .Build(),
                })
                .Build());

            Assert.IsTrue(r.Status == Status.Success,r.Exception?.ToString());
            Assert.IsNotNull(r.Data);

            var hasR = service.HasCollection(HasCollectionParam.NewBuilder()
                .WithCollcetionName(collectionName)
                .Build());

            Assert.IsTrue(hasR.Status == Status.Success);
            Assert.IsTrue(hasR.Data);
        }
    }
}