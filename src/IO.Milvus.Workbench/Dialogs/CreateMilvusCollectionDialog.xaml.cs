using IO.Milvus.Workbench.Utils;
using IO.Milvus.Workbench.ViewModels;
using System.Windows;

namespace IO.Milvus.Workbench.Dialogs
{
    /// <summary>
    /// CreateCollectionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CreateMilvusCollectionDialog : Window
    {
        public CreateMilvusCollectionDialog(string connectionName)
        {
            InitializeComponent();
            DataContext = Vm = new CreateMilvusCollectionDialogViewModel(connectionName);
            Vm.CloseAction = (result) =>
            {
                DialogResult = result;
            };

            this.SetOwnerWindow();
        }

        public CreateMilvusCollectionDialogViewModel Vm { get; }
    }
}
