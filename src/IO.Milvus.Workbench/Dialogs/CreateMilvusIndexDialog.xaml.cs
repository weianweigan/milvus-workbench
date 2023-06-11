using IO.Milvus.ApiSchema;
using IO.Milvus.Workbench.Utils;
using IO.Milvus.Workbench.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace IO.Milvus.Workbench.Dialogs;

/// <summary>
/// CreateMilvusIndexDialog.xaml 的交互逻辑
/// </summary>
public partial class CreateMilvusIndexDialog : Window
{
    public CreateMilvusIndexDialog(string collectionName,string fieldName)
    {
        InitializeComponent();
        Vm = new CreateMilvusIndexDialogViewModel(collectionName, fieldName);
        DataContext = Vm;
        Vm.CloseAction = (result) =>
        {
            DialogResult = result;
        };

        this.SetOwnerWindow();
    }

    public CreateMilvusIndexDialogViewModel Vm { get; }
}

public class MilvusIndexModel
{
    public string CollectionName { get; set; }

    public string FieldName { get; set; }

    public string IndexName { get; set;}

    public MilvusIndexType IndexType { get; set; } = MilvusIndexType.BIN_IVF_FLAT;

    public MilvusMetricType MetricType { get; set; } = MilvusMetricType.IP;
}

public partial class CreateMilvusIndexDialogViewModel : DialogViewModel
{
    public CreateMilvusIndexDialogViewModel(string collectionName, string fieldName)
    {
        Indexes = new ObservableCollection<MilvusIndexModel>()
        {
            new MilvusIndexModel()
            {
                CollectionName = collectionName,
                FieldName = fieldName,
                IndexName = Constants.DEFAULT_INDEX_NAME
            }
        };
    }

    public ObservableCollection<MilvusIndexModel> Indexes { get; set; }

    protected override void AddClick()
    {
        CloseAction?.Invoke(true);
    }
}