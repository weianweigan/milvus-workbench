using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.MilvusTests.Client.Base;
using IO.Milvus.Param.Alias;
using IO.MilvusTests;

namespace IO.Milvus.Client.Tests
{
    [TestClass]
    public class AliasTests : MilvusServiceClientTestsBase
    {
        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName,HostConfig.DefaultAliasName)]
        public void ACreateAliasTest(string collectionName,string aliasName)
        {
            var r = MilvusClient.CreateAlias(CreateAliasParam.Create(collectionName, aliasName));
            AssertRpcStatus(r);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultAliasName)]
        public void BAlterAliasTest(string collectionName, string aliasName)
        {
            //TODO: alter to another collection,not self.
            var r = MilvusClient.AlterAlias(AlterAliasParam.Create(collectionName, aliasName));
            AssertRpcStatus(r);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultAliasName)]
        public void CDropAliasTest(string collectionName, string aliasName)
        {
            var r = MilvusClient.DropAlias(DropAliasParam.Create(collectionName, aliasName));
            AssertRpcStatus(r);
        }
    }
}
