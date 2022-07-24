using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Utils;
using System;
using System.Windows;

namespace IO.Milvus.Workbench.ViewModels
{
    public class AddMilvusDailogViewModel : ObservableObject
    {
        private RelayCommand _addCmd;
        private RelayCommand _canacelCmd;

        public string Name { get; set; } = "Test";

        public string Host { get; set; } = "192.168.100.139";

        public int Port { get; set; } = 19530;

        public Action<bool> CloseAction { get;internal set; }

        public RelayCommand AddCmd { get => _addCmd = (_addCmd = new RelayCommand(AddClick)); }

        public RelayCommand CanacelCmd { get => _canacelCmd  = (_canacelCmd = new RelayCommand(CancelClick)); }

        private void CancelClick()
        {
            CloseAction?.Invoke(false);
        }

        private void AddClick()
        {
            if (!IPValidationUtils.IsHost(Host))
            {
                MessageBox.Show("Host Error");
                return;
            }
            if (!PortValidationUtils.PortInRange(Port))
            {
                MessageBox.Show("Port Error");
                return;
            }

            CloseAction?.Invoke(true);
        }
    }
}
