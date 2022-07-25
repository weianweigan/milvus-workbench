using IO.Milvus.Client;
using IO.Milvus.Param;
using IO.Milvus.Param.Collection;
using IO.Milvus.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace IO.Milvus.Workbench.Models
{
    //TODO: Fix JsonIngnore in net461
    public class MilvusConnectionNode : Node<CollectionNode>,IEqualityComparer<MilvusConnectionNode>
    {

        public MilvusConnectionNode(string name, string host, int port)
        {
            Name = name;
            Host = host;
            Port = port;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public MilvusServiceClient ServiceClient { get; private set; }

        public string DisplayName => string.IsNullOrEmpty(Name) ? Url : $"{Name}({Url})";

        public string Url => $"{Host}:{Port}";

        public async Task ConnectAsync()
        {
            Application.Current.Dispatcher.Invoke(() => State = NodeState.Connecting);

            try
            {
                if (ServiceClient != null)
                {
                    ServiceClient.Close();
                    ServiceClient = null;
                }

                var connect = ConnectParam.Create(Host, Port);
                ServiceClient = new MilvusServiceClient(connect);

                var r = ServiceClient.ShowCollections(ShowCollectionsParam.Create(null, Grpc.ShowType.All));
                if (r.Status != Status.Success)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        State = NodeState.Error;
                        Msg = $"{r.Status}: {r.Exception.Message}";
                    });
                }

                if (r.Data.CollectionNames.IsEmpty())
                {
                    return;
                }

                for (int i = 0; i < r.Data.CollectionNames.Count; i++)
                {
                    var collectionNode = new CollectionNode(
                        this,
                        r.Data.CollectionNames[i],
                        r.Data.CollectionIds[i],
                        r.Data.CreatedTimestamps[i],
                        r.Data.CreatedUtcTimestamps[i]);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Children.Add(collectionNode);
                    });

                    await collectionNode.ConnectAsync();
                }
            }
            catch (System.Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    State = NodeState.Error;
                    Msg = ex.Message;
                });
            }
        }

        public bool Equals(MilvusConnectionNode x, MilvusConnectionNode y)
        {
            return x.Url == y.Url;
        }

        public int GetHashCode(MilvusConnectionNode obj)
        {
            return obj.Url.GetHashCode();
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
