using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Mvvm;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using System.Collections.Generic;
using System;
using Milvus.Workbench.ViewModels.Tools;
using Milvus.Workbench.ViewModels.Documents;
using CommunityToolkit.Mvvm.Input;

namespace Milvus.Workbench.ViewModels;

public class DockFactory : Factory
{
    private readonly object _context;
    private IRootDock? _rootDock;
    private ToolDock? _nodesDock;
    private IDocumentDock? _documentDock;

    public DockFactory(object context)
    {
        _context = context;
    }

    public NodesManagerViewModel? NodesManager { get; private set; }

    public override IRootDock CreateLayout()
    {
        var nodesManagerViewModel = new NodesManagerViewModel() { Id = nameof(NodesManagerViewModel), Title = "Milvus Instances", CanClose = false, CanFloat = false };
        var nodesManagerDock = new ToolDock
        {
            ActiveDockable = nodesManagerViewModel,
            VisibleDockables = CreateList<IDockable>(nodesManagerViewModel),
            Alignment = Alignment.Left,
            Proportion = 0.25,
        };

        var welcomeDock = new WelcomeViewModel() { Id = nameof(WelcomeViewModel), Title = "Welcome"};

        var documentDock = new DocumentDock
        {
            CanClose = true,
            IsCollapsable = false,
            CanCreateDocument = true,
            ActiveDockable = welcomeDock,
            VisibleDockables = CreateList<IDockable>(welcomeDock),
            CreateDocument = new RelayCommand(() => { }, () => false)
        }; 

        var mainLayout = new ProportionalDock
        {
            CanClose = false,
            CanFloat = true,
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>
            (
                nodesManagerDock,
                new ProportionalDockSplitter(),
                documentDock
            ),
        };

        var rootDock = CreateRootDock();

        rootDock.IsCollapsable = false;
        rootDock.ActiveDockable = nodesManagerDock;
        rootDock.DefaultDockable = mainLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(mainLayout);

        _documentDock = documentDock;
        _rootDock = rootDock;
        _nodesDock = nodesManagerDock;

        NodesManager = nodesManagerViewModel;

        return rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>
        {
            ["Nodes"] = () => _nodesDock,
            ["MainLayout"] = () => layout,
        };

        DockableLocator = new Dictionary<string, Func<IDockable?>>()
        {
            ["Root"] = () => _rootDock,
            ["Documents"] = () => _documentDock
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }
}
