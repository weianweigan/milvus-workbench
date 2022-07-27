using CommunityToolkit.Mvvm.ComponentModel;
using IO.Milvus.Workbench.Models;

namespace IO.Milvus.Workbench.ViewModels
{
    public class ImportDataPageViewModel : ObservableObject
    {
        public ImportDataPageViewModel(MilvusConnectionNode connection, CollectionNode collection, PartitionNode partition)
        {
            Connection = connection;
            SelectedCollection = collection;
            SelectedPartition = partition;
        }

        public MilvusConnectionNode Connection { get; }

        public CollectionNode SelectedCollection { get; }

        public PartitionNode SelectedPartition { get; }
    }
}
