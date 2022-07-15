using IO.Milvus.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.MilvusTests.Client.Base;
using IO.Milvus.Param.Dml;

namespace IO.Milvus.Client.Tests
{
    /// <summary>
    /// a test class to execute unittest about data process
    /// </summary>
    [TestClass]
    public class DataTests : MilvusServiceClientTestsBase
    {
        private void PreaeData()
        {
            var r = new Random(DateTime.Now.Second);

            var bookIds = new List<long>();
            var wordCounts = new List<long>();
            var bookIntros = new List<List<float>>();

            for (int i = 0; i < 2000; i++)
            {
                bookIds.Add(i);
                wordCounts.Add(i + 10000);
                List<float> vector = new List<float>();
                for (int k = 0; k < 2; ++k)
                {
                    vector.Add(r.NextSingle());
                }
                bookIntros.Add(vector);
            }

            var insertParam = InsertParam.Create("", "",
                new Google.Protobuf.Collections.RepeatedField<Grpc.FieldData>()
                {
                    new Grpc.FieldData()
                    {
                        FieldName = "book_id",
                        Type = Grpc.DataType.Int64,
                        Vectors = new Grpc.VectorField()
                        {
                            
                        }
                    }
                },
                1);
        }

        [TestMethod()]
        public void InsertTest()
        {
            
        }

        [TestMethod()]
        public void InsertAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FlushTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FlushAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetFlushStateTest()
        {
            Assert.Fail();
        }
    }
}
