using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using Milvus.Workbench.Models;
using Milvus.Workbench.Utils;
using Milvus.Workbench.ViewModels.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milvus.Workbench.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private IRootDock? _layout;

    private DockFactory _factory;
    private AsyncRelayCommand? _loadCommand;

    public MainWindowViewModel()
    {
        _factory = new DockFactory(this);

        _layout = _factory.CreateLayout();
        if (Layout is { })
        {
            _factory.InitLayout(Layout);

            if (Layout is { } root)
            {
                root.Navigate.Execute(nameof(NodesManagerViewModel));
            }
        }
    }


    public AsyncRelayCommand LoadCommand { get => _loadCommand ??= new AsyncRelayCommand(LoadAsync); }

    private async Task LoadAsync()
    {
        var vectorDbs = await VectorDbConfigUtils.ReadConfigAsync();

        foreach (var vectorDb in vectorDbs)
        {
            var milvusVectorDb = new MilvusVectorDbNode(vectorDb);
            _factory.NodesManager.Nodes.Add(milvusVectorDb);
            milvusVectorDb.LoadAsync();
        }
    }
}