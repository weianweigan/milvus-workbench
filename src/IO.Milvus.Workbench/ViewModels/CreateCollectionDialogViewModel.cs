using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IO.Milvus.Workbench.Models.Fields;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace IO.Milvus.Workbench.ViewModels
{
    public class CreateCollectionDialogViewModel : ObservableObject
    {
        private RelayCommand _addCmd;
        private RelayCommand _canacelCmd;
        private RelayCommand _addFieldComd;
        private RelayCommand<Field> _removeFieldCmd;

        public CreateCollectionDialogViewModel(string connectionName)
        {
            Title = connectionName;
        }

        public Action<bool> CloseAction { get; internal set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public ObservableCollection<Field> Fields { get; set; } = new ObservableCollection<Field>()
        {
            new PrimaryField(),
            new VectorField()
        };

        public RelayCommand AddFieldComd { get => _addFieldComd ?? (_addFieldComd = new RelayCommand(AddFieldClick)); }

        public RelayCommand<Field> RemoveFieldCmd { get => _removeFieldCmd ?? (_removeFieldCmd = new RelayCommand<Field>(RemoveFieldClick)); }

        public RelayCommand AddCmd { get => _addCmd = (_addCmd = new RelayCommand(AddClick)); }

        public RelayCommand CanacelCmd { get => _canacelCmd = (_canacelCmd = new RelayCommand(CancelClick)); }

        private void RemoveFieldClick(Field obj)
        {
            Fields.Remove(obj);
        }

        private void AddFieldClick()
        {
            Fields.Add(new DefaultField());
        }

        private void CancelClick()
        {
            CloseAction?.Invoke(false);
        }

        private void AddClick()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Name Canot be Null");
                return;
            }

            //TODO: Validate Fields
            foreach (var field in Fields)
            {
                
            }
            
            CloseAction?.Invoke(true);
        }
    }
}
