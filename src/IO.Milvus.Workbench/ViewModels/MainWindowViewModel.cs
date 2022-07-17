using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.Milvus.Workbench.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private MilvusServiceModel _selectedServiceHost;

        public MainWindowViewModel()
        {
            ServiceHosts = new ObservableCollection<MilvusServiceModel>()
            {
                new MilvusServiceModel()
                {
                    Host = "192.168.100.139",
                    Port = 19530
                },
            };
        }

        public ObservableCollection<MilvusServiceModel> ServiceHosts { get; set; }

        public MilvusServiceModel SelectedServiceHost {
            get => _selectedServiceHost; 
            set => SetProperty(ref _selectedServiceHost, value); 
        }

        public RelayCommand AddCmd { get; set; }

        public RelayCommand DeleteCmd { get; set; }
    }

    public class HostOverviewViewModel : ObservableObject
    {

    }
}
