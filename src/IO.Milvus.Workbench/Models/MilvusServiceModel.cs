using CommunityToolkit.Mvvm.ComponentModel;

namespace IO.Milvus.Workbench.Models
{
    public class MilvusServiceModel : ObservableObject
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public string DisplayName => string.IsNullOrEmpty(Name) ? Host : Name;

        public int Port { get; set; }

        public string Url => $"{Host}:{Port}";

        public bool IsSelected { get; set; }
    }

    
}
