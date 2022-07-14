using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.Milvus.Param;

namespace IO.Milvus.Client.Tests
{
    [TestClass()]
    public partial class MilvusServiceClientTests
    {
        private static MilvusServiceClient milvusclient;

        [TestMethod()]
        public void MilvusServiceClientTest()
        {
            var service = DefaultClient();

            Assert.IsNotNull(service);
            Assert.IsTrue(service.ClientIsReady());
        }
         
        public static MilvusServiceClient DefaultClient()
        {
            milvusclient ??= new MilvusServiceClient(
                ConnectParam.Create(
                    host: "192.168.100.139",
                    port: 19530));

            Assert.IsNotNull(milvusclient);
            Assert.IsTrue(milvusclient.ClientIsReady());

            return milvusclient;
        }

        public static MilvusServiceClient NewClient()
        {
            var client = new MilvusServiceClient(
                ConnectParam.Create(
                    host: "192.168.100.139",
                    port: 19530));

            Assert.IsNotNull(client);
            Assert.IsTrue(client.ClientIsReady());

            return client;
        }

        public static MilvusServiceClient NewErrorClient()
        {
            var client = new MilvusServiceClient(
                ConnectParam.Create(
                    host: "192.168.100.139",
                    port: 19531));

            Assert.IsNotNull(client);
            Assert.IsTrue(client.ClientIsReady());

            return client;
        }

        [TestMethod()]
        public void CloseTest()
        {
            var service = NewClient();
            service.Close();
        }

        [TestMethod()]
        public async Task CloseAsyncTest()
        {
            var service = NewClient();
            await service.CloseAsync();
        }
    }

}