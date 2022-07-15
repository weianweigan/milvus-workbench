using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.Milvus.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO.MilvusTests.Client.Base;
using IO.Milvus.Param.Partition;
using IO.MilvusTests;
using IO.Milvus.Param;

namespace IO.Milvus.Client.Tests
{
    [TestClass()]
    public class PartitionTest:MilvusServiceClientTestsBase
    {
        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName,HostConfig.DefaultTestPartitionName)]
        public void ACreatePartitionTest(string collectionName,string partitionName)
        {
            var r = Milvusclient.CreatePartition(CreatePartitionParam.Create(
                collectionName, partitionName));

            AssertRpcStatus(r);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultTestPartitionName)]
        public void BLoadPartitionsTest(string collectionName, string partitionName)
        {
            var r = Milvusclient.LoadPartitions(LoadPartitionsParam.Create(
                collectionName, partitionName));

            AssertRpcStatus(r);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultTestPartitionName)]
        public void CHasPartitionTest(string collectionName, string partitionName)
        {
            var r = Milvusclient.HasPartition(HasPartitionParam.Create(
                collectionName, partitionName));

            AssertRBool(r);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultTestPartitionName)]
        public void DGetPartitionStatisticsTest(string collectionName, string partitionName)
        {
            var r = Milvusclient.GetPartitionStatistics(GetPartitionStatisticsParam.Create(
                        collectionName, partitionName));

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Stats.Count > 0);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultTestPartitionName)]
        public void EShowPartitionsTest(string collectionName, string partitionName)
        {
            var r = Milvusclient.ShowPartitions(ShowPartitionsParam.Create(
            collectionName,null));

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.Status.ErrorCode == Grpc.ErrorCode.Success);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultTestPartitionName)]
        public void FReleasePartitionsTest(string collectionName, string partitionName)
        {
            var r = Milvusclient.ReleasePartitions(ReleasePartitionsParam.Create(
                collectionName, partitionName));

            AssertRpcStatus(r);
        }

        [TestMethod()]
        [DataRow(HostConfig.DefaultTestCollectionName, HostConfig.DefaultTestPartitionName)]
        public void GDropPartitionTest(string collectionName, string partitionName)
        {
            var r = Milvusclient.DropPartition(DropPartitionParam.Create(
                    collectionName, partitionName));

            AssertRpcStatus(r);
        }
        
    }
}
