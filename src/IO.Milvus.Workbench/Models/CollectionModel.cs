using CommunityToolkit.Mvvm.ComponentModel;

namespace IO.Milvus.Workbench.Models
{
    public class CollectionModel:ObservableObject
    {
        public string Name { get; set; }

        public bool Loaded { get; set; }

        public int EntitiesCount { get; set; }

    }

    
}
