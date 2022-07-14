using IO.Milvus.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.Milvus.Param;
using IO.Milvus.Param.Collection;

namespace IO.Milvus.Client.Tests
{
    public partial class MilvusServiceClientTests
    {
        public const string DefaultTestCollectionName = "test";

        [TestMethod()]
        [DataRow(DefaultTestCollectionName, true)]
        [DataRow("test2", false)]
        public void HasCollectionTest(string collectionName, bool exist)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.HasCollection(collectionName);

            Assert.IsTrue(r.Status == Status.Success);
            Assert.IsTrue(r.Data == exist);
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public void HasCollectionErrorTest(string collectionName)
        {
            var milvusClient = DefaultClient().WithTimeout(TimeSpan.FromMilliseconds(5));
            var r = milvusClient.HasCollection(collectionName);

            Assert.IsTrue(r.Status == Status.Success,r.Exception?.ToString());
        }

        [TestMethod()]
        public void CreateCollectionTest()
        {
            var rd = new Random(DateTime.Now.Second);
            var collectionName = $"test{rd.Next()}";
            var milvusClient = DefaultClient();

            var r = milvusClient.CreateCollection(
                CreateCollectionParam.Create(
                   collectionName: collectionName,
                   shardsNum: 2,
                   new List<FieldType>()
                {
                    FieldType.Create(
                        $"priKey{rd.Next()}",
                    Grpc.DataType.Int64,
                    null,
                    true
                    )
                }));

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsNotNull(r.Data);

            var hasR = milvusClient.HasCollection(HasCollectionParam.Create(collectionName));
            Assert.IsTrue(hasR.Status == Status.Success);
            Assert.IsTrue(hasR.Data);

            milvusclient.DropCollection(collectionName);

            hasR = milvusClient.HasCollection(HasCollectionParam.Create(collectionName));
            Assert.IsTrue(hasR.Status == Status.Success);
            Assert.IsTrue(!hasR.Data);
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public void ShowCollectionsTest(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.ShowCollections(ShowCollectionsParam.Create(null));

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.CollectionNames.Contains(collectionName));
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public void DescribeCollectionTest(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.DescribeCollection(DescribeCollectionParam.Create(collectionName));

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Schema.Name == collectionName);
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public void DescribeCollectionWithNameTest(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.DescribeCollection(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Schema.Name == collectionName);
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName, true)]
        [DataRow(DefaultTestCollectionName, false)]
        public void GetCollectionStatisticsTest(string collectionName,bool isFlushCollection)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.GetCollectionStatistics(GetCollectionStatisticsParam.Create(collectionName,isFlushCollection));

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Stats.First().Key == "row_count");
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public void LoadCollectionTest(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.LoadCollection(LoadCollectionParam.Create(collectionName));

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);

            r = milvusClient.ReleaseCollection(ReleaseCollectionParam.Create(collectionName));

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);
        }

        [TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public void LoadCollectionWithNameTest(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = milvusClient.LoadCollection(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);

            r = milvusClient.ReleaseCollection(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);
        }

        //[TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public async Task LoadCollectionAsyncTestAsync(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = await milvusClient.LoadCollectionAsync(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);

            r = await milvusClient.ReleaseCollectionAsync(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);
        }

        //[TestMethod()]
        [DataRow(DefaultTestCollectionName)]
        public async Task LoadCollectionAsyncWithNameTestAsync(string collectionName)
        {
            var milvusClient = DefaultClient();
            var r = await milvusClient.LoadCollectionAsync(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);

            r = await milvusClient.ReleaseCollectionAsync(collectionName);

            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Msg == RpcStatus.SUCCESS_MSG);
        }
    }
}
