using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using Milvus.Workbench.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Milvus.Workbench.ViewModels.Tools;

public partial class NodesManagerViewModel : Tool
{
    public ObservableCollection<INode> Nodes { get; private set; } = new();
}