using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IO.Milvus.Workbench.Models;

public partial class CollectionNode
{
    private AsyncRelayCommand<ListView> _searchCmd;
    private string _searchText;
    private DataTable _searchResultData;
    private string _searchVector;
    private List<MilvusFieldTypeModel> _searchField;
    private MilvusFieldTypeModel _selectedField;

    public AsyncRelayCommand<ListView> SearchCmd { get => _searchCmd ??= new AsyncRelayCommand<ListView>(SearchClickAsync,(v) => CanSearch()); }

    public string SearchVector
    {
        get => _searchVector; set
        {
            SetProperty(ref _searchVector , value);
        }
    }

    public string SearchText
    {
        get => _searchText; set
        {
            SetProperty(ref _searchText, value);
            SearchCmd.NotifyCanExecuteChanged();
        }
    }

    public int TopK { get; set; } = 50;

    public int Nprobe { get; set; } = 10;

    public int RoundDecimal { get; set; } = 4;

    public DataTable SearchResultData { get => _searchResultData; set => SetProperty(ref _searchResultData , value) ; }

    public List<MilvusFieldTypeModel> SearchFields { get => _searchField; set => SetProperty(ref _searchField, value); }

    public MilvusFieldTypeModel SelectedField { get => _selectedField; set => SetProperty(ref _selectedField , value); }

    private bool CanSearch()
    {
        return !string.IsNullOrEmpty(SearchVector);
    }

    private async Task SearchClickAsync(ListView listView ,CancellationToken cancellationToken)
    {
        try
        {
            var searchField = Fields.Where(p =>
                p.DataType == MilvusDataType.FloatVector ||
                p.DataType == MilvusDataType.BinaryVector)
                .FirstOrDefault();

            if (searchField == null)
            {
                return;
            }

            var vectors = JsonSerializer.Deserialize<List<List<float>>>(SearchVector);

            var searchParameter = MilvusSearchParameters.Create(
                Name, searchField.Name, this.Fields.Where(p => p != searchField).Select(p => p.Name).ToList());
            searchParameter
                .WithVectors(vectors)
                .WithConsistencyLevel(MilvusConsistencyLevel.Strong)
                .WithTopK(TopK)
                .WithMetricType(MilvusMetricType.L2)
                .WithParameter("nporbe", Nprobe.ToString())
                .WithRoundDecimal(RoundDecimal);

            if (!string.IsNullOrEmpty(SearchText))
                searchParameter.WithExpr(SearchText);

            var result = await Parent.MilvusClient.SearchAsync(
                searchParameter,
                cancellationToken
                    );

            SearchResultData = ToDataTable(listView, result.Results.FieldsData);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void GenerateRandomSearchVectors()
    {
        SearchFields = Fields.Where(p => p.DataType == MilvusDataType.FloatVector || p.DataType == MilvusDataType.BinaryVector).ToList();
        SelectedField = SearchFields.FirstOrDefault();
        if (SelectedField == null)
        {
            return;
        }

        var random = new Random();
        List<float> floats = new List<float>();
        for (int i = 0; i < SelectedField.Dimension; i++)
        {
            floats.Add((float)Math.Round(random.NextDouble(), 2));
        }

        SearchVector = JsonSerializer.Serialize(new List<List<float>>() { floats });
    }
}
