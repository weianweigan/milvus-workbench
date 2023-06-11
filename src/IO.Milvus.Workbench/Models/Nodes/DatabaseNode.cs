using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Client;
using IO.Milvus.Client.gRPC;
using IO.Milvus.Workbench.Dialogs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IO.Milvus.Workbench.Models
{
    public class DatabaseNode : Node<CollectionNode>
    {
        #region Fields
        private IAsyncRelayCommand _deleteCmd;
        private RelayCommand _disconnectCmd;
        private IAsyncRelayCommand _connectCmd;
        private IAsyncRelayCommand _createCollectionCmd;
        #endregion

        #region Ctor
        public DatabaseNode(
            VectorDbManagerNode parent, 
            VectorDatabaseInstanceConfig dbConfig)
        { 
            Parent = parent;
            DbConfig = dbConfig;
        }
        #endregion

        #region Properties
        public VectorDbManagerNode Parent { get; }

        public string Host { get; set; }

        public int Port { get; set; }

        public IMilvusClient MilvusClient { get; private set; }

        public string DisplayName => DbConfig.Name;

        public string Url => $"{Host}:{Port}";

        public IAsyncRelayCommand DeleteCmd { get => _deleteCmd ?? (_deleteCmd = new AsyncRelayCommand(DeleteAsync)); }

        public RelayCommand DisconnectCmd { get => _disconnectCmd ?? (_disconnectCmd = new RelayCommand(Disconnect, () => State == NodeState.Success)); }

        public IAsyncRelayCommand ConnectCmd { get => _connectCmd ?? (_connectCmd = new AsyncRelayCommand(RefreshAsync, () => State.CanConnect())); }

        public IAsyncRelayCommand CreateCollectionCmd { get => _createCollectionCmd ?? (_createCollectionCmd = new AsyncRelayCommand(CreateCollectionClickAsync, () => State == NodeState.Success)); }

        public VectorDatabaseInstanceConfig DbConfig { get; }
        #endregion

        #region Public Methods
        public async Task RefreshAsync()
        {
            Disconnect();

            await ConnectAsync();
        }

        public override string ToString()
        {
            return State == NodeState.Success ? DisplayName : Msg;
        }
        #endregion

        #region Private Methods
        protected override void OnStateChanged()
        {
            DisconnectCmd.NotifyCanExecuteChanged();
            ConnectCmd.NotifyCanExecuteChanged();
            CreateCollectionCmd.NotifyCanExecuteChanged();
        }

        private async Task DeleteAsync()
        {
            Children.Clear();
            MilvusClient?.Close();
            Parent.Children.Remove(this);

            await Parent.SaveAsync();
        }

        private async Task CreateCollectionClickAsync()
        {
            //Input collection name
            var dialog = new CreateMilvusCollectionDialog(DisplayName);
            if (dialog.ShowDialog() == true)
            {
                //Check if collection existed
                bool has = Children.Any(p => p.Name == dialog.Vm.Name);
                if (has)
                {
                    MessageBox.Show($"{dialog.Vm.Name} Exist");
                    return;
                }

                try
                {
                    await MilvusClient.CreateCollectionAsync(
                        dialog.Vm.Name,
                        dialog.Vm.Fields.Select(p => p.ToFieldType()).ToList());

                    await RefreshAsync();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Disconnect()
        {
            Children.Clear();
            if (State == NodeState.Success)
            {
                MilvusClient?.Close();
            }
            MilvusClient = null;

            State = NodeState.Closed;
        }

        public async Task ConnectAsync()
        {
            State = NodeState.Connecting;

            try
            {
                if (MilvusClient != null)
                {
                    MilvusClient.Close();
                    MilvusClient = null;
                }

                MilvusClient = new MilvusGrpcClient(DbConfig.Host, DbConfig.Port, DbConfig.Username, DbConfig.Password);

                var helathState = await MilvusClient.HealthAsync();
                if (!helathState.IsHealthy)
                {
                    State = NodeState.Error;
                    Msg = $"{helathState}";
                    return;
                }

                IList<MilvusCollection> collections = await MilvusClient.ShowCollectionsAsync();

                if (collections?.Any() == true)
                {
                    foreach (var collection in collections)
                    {
                        var collectionNode = new CollectionNode(this, collection.CollectionName, collection.CollectionId);
                        Children.Add(collectionNode);
                        await collectionNode.ConnectAsync();
                    }
                }

                State = NodeState.Success;
            }
            catch (System.Exception ex)
            {
                State = NodeState.Error;
                Msg = ex.Message;
            }
        }
        #endregion
    }
}
