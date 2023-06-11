using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace IO.Milvus.Workbench.ViewModels;


public partial class CreateMilvusCollectionDialogViewModel : DialogViewModel
{
    private RelayCommand _addFieldComd;
    private MilvusFieldTypeModel _selectedFieldType;

    public CreateMilvusCollectionDialogViewModel(string connectionName)
    {
        Title = connectionName;
    }

    public string Name { get; set; } = "Collection1";

    public string Description { get; set; }

    public string Title { get; set; }

    public ObservableCollection<MilvusFieldTypeModel> Fields { get; set; } = new()
    {
        new MilvusFieldTypeModel("id",MilvusDataType.VarChar,true,0,null){MaxLength = 100},
        new MilvusFieldTypeModel("floatVector",MilvusDataType.FloatVector,false,0,null){Dimension = 1536}
    };

    public MilvusFieldTypeModel SelectedFieldType
    {
        get => _selectedFieldType; set
        {
            if(SetProperty(ref _selectedFieldType, value))
            {
                RemoveFieldCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public RelayCommand AddFieldComd { get => _addFieldComd ?? (_addFieldComd = new RelayCommand(AddField)); }

    [RelayCommand(CanExecute =nameof(CanRemoveField))]
    private void RemoveField(MilvusFieldTypeModel obj)
    {
        Fields.Remove(obj);
    }

    private bool CanRemoveField()
    {
        if (SelectedFieldType == null)
        {
            return false;
        }
        return SelectedFieldType.IsPrimaryKey == false && 
            SelectedFieldType.DataType != MilvusDataType.FloatVector && 
            SelectedFieldType.DataType != MilvusDataType.BinaryVector;
    }

    [RelayCommand]
    private void AddField()
    {
        Fields.Add(new MilvusFieldTypeModel("field", MilvusDataType.Float, false, 0, null));
    }

    protected override void AddClick()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            MessageBox.Show("Name Cannot be Null");
            return;
        }

        //TODO: Validate Fields
        foreach (var field in Fields)
        {
            var newField = field.ToFieldType();
        }

        CloseAction?.Invoke(true);
    }
}
