using AvalonDock.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Dialogs;
using IO.Milvus.Workbench.DocumentViews;
using IO.Milvus.Workbench.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IO.Milvus.Workbench.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private AsyncRelayCommand _addCmd;
        private MilvusManagerNode _milvusManagerNode;
        private RelayCommand _openPageCmd;

        public MainWindowViewModel(LayoutDocumentPane documentPane)
        {
            _milvusManagerNode = new MilvusManagerNode();
            DocumentPane = documentPane;
        }

        public MilvusManagerNode MilvusManagerNode { get => _milvusManagerNode; set => SetProperty(ref _milvusManagerNode, value); }

        public Node SelectedNode { get;private set; }

        public AsyncRelayCommand AddCmd { get => _addCmd ?? (_addCmd = new AsyncRelayCommand(AddClickAsync)); }

        public RelayCommand OpenPageCmd { get => _openPageCmd ?? (_openPageCmd = new RelayCommand(OpenClick)); }

        private void OpenClick()
        {
            SelectedNode = MilvusManagerNode
                .ListAllNode()
                .FirstOrDefault(p => p.IsSelected);
            if (SelectedNode == null)
            {
                return;
            }

            var existDoc = DocumentPane.Children.FirstOrDefault(p => (p.Content as UserControl)?.DataContext == SelectedNode);
            if (existDoc != null)
            {
                existDoc.IsActive = true;
                return;
            }
           

            if (SelectedNode is CollectionNode node)
            {
                var newDocPage = new LayoutDocument()
                {
                    Title = node.Name,
                    Content = new Frame()
                    {
                        Content = new CollectionPage()
                        {
                            DataContext = node,
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
                var milvus = new MilvusConnectionNode(
                    dialog.Vm.Name,
                    dialog.Vm.Host,
                    dialog.Vm.Port);
                MilvusManagerNode.Children.Add(milvus);

                await milvus.ConnectAsync();
            }
        }

        public RelayCommand DeleteCmd { get; set; }
        public LayoutDocumentPane DocumentPane { get; }
    }
}
