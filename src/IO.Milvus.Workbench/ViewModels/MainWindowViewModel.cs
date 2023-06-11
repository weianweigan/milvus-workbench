using AvalonDock.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Dialogs;
using IO.Milvus.Workbench.DocumentViews;
using IO.Milvus.Workbench.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IO.Milvus.Workbench.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private AsyncRelayCommand _addCmd;
    private VectorDbManagerNode _milvusManagerNode;

    public MainWindowViewModel(LayoutDocumentPane documentPane)
    {
        _milvusManagerNode = new VectorDbManagerNode(this);
        DocumentPane = documentPane;
    }

    #region Properties
    public LayoutDocumentPane DocumentPane { get; }

    public VectorDbManagerNode MilvusManagerNode { get => _milvusManagerNode; set => SetProperty(ref _milvusManagerNode, value); }

    public Node SelectedNode { get; private set; }

    public AsyncRelayCommand AddCmd { get => _addCmd ?? (_addCmd = new AsyncRelayCommand(AddClickAsync)); }
    #endregion

    #region Private Methods
    [RelayCommand]
    internal void Open()
    {
        SelectedNode = MilvusManagerNode
            .ListAllNode()
            .FirstOrDefault(p => p.IsSelected);
        if (SelectedNode == null)
        {
            return;
        }

        var existDoc = DocumentPane.Children.FirstOrDefault(p => p.Title == SelectedNode.Name && ((p.Content as Frame).Content as Page)?.DataContext == SelectedNode);
        if (existDoc != null)
        {
            existDoc.IsActive = true;
            return;
        }


        if (SelectedNode is CollectionNode collectionNode)
        {
            var newDocPage = new LayoutDocument()
            {
                Title = collectionNode.Name,
                Content = new Frame()
                {
                    Content = new CollectionPage()
                    {
                        DataContext = collectionNode,
                    }
                },
            };

            DocumentPane.Children.Add(newDocPage);
            newDocPage.IsActive = true;
        }else if (SelectedNode is PartitionNode partitionNode)
        {
            var newDocPage = new LayoutDocument()
            {
                Title = partitionNode.Name,
                Content = new Frame()
                {
                    Content = new CollectionPage()
                    {
                        DataContext = partitionNode.Parent,
                    }
                },
            };

            DocumentPane.Children.Add(newDocPage);
            newDocPage.IsActive = true;
        }
    }

    [RelayCommand]
    internal void Search()
    {
        SelectedNode = MilvusManagerNode
            .ListAllNode()
            .FirstOrDefault(p => p.IsSelected);
        if (SelectedNode == null)
        {
            return;
        }

        var existDoc = DocumentPane.Children.FirstOrDefault(p => p.Title == $"Search:{SelectedNode.Name}" && ((p.Content as Frame).Content as Page)?.DataContext == SelectedNode);
        if (existDoc != null)
        {
            existDoc.IsActive = true;
            return;
        }

        if (SelectedNode is CollectionNode collectionNode)
        {
            collectionNode.GenerateRandomSearchVectors();
            var newDocPage = new LayoutDocument()
            {
                Title = $"Search:{collectionNode.Name}",
                Content = new Frame()
                {
                    Content = new VectorSearchPage()
                    {
                        DataContext = collectionNode,
                        vectorTextEditor = { Text = collectionNode.SearchVector }
                    }
                },
            };

            DocumentPane.Children.Add(newDocPage);
            newDocPage.IsActive = true;
        }
        else if (SelectedNode is PartitionNode partitionNode)
        {
            partitionNode.Parent.GenerateRandomSearchVectors();
            var newDocPage = new LayoutDocument()
            {
                Title = $"Search:{partitionNode.Parent.Name}",
                Content = new Frame()
                {
                    Content = new VectorSearchPage()
                    {
                        DataContext = partitionNode.Parent,
                        vectorTextEditor = { Text = partitionNode.Parent.SearchVector }
                    }
                },
            };

            DocumentPane.Children.Add(newDocPage);
            newDocPage.IsActive = true;
        }
    }

    private async Task AddClickAsync()
    {
        var dialog = new AddMilvusDialog();
        if (dialog.ShowDialog() == true)
        {
            var milvus = new DatabaseNode(
                MilvusManagerNode,
                new VectorDatabaseInstanceConfig(dialog.Vm.Name,dialog.Vm.Host,dialog.Vm.Port,dialog.Vm.Username,dialog.Vm.Password));
            MilvusManagerNode.Children.Add(milvus);

            await milvus.ConnectAsync();
        }

        //Save Config
        await MilvusManagerNode.SaveAsync();
    }
    #endregion

    #region Internal Methods
    internal async Task LoadMilvusInstanceConfigAsync()
    {
        var configs = await MilvusManagerNode.ReadConfigAsync();

        foreach (var config in configs)
        {
            var milvus = new DatabaseNode(
                MilvusManagerNode,
                config);
            MilvusManagerNode.Children.Add(milvus);

            await milvus.ConnectAsync();
        }
    }
    #endregion
}
