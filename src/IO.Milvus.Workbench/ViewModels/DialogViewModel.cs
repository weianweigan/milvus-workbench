using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace IO.Milvus.Workbench.ViewModels;

public abstract class DialogViewModel : ObservableObject
{
    private RelayCommand _addCmd;
    private RelayCommand _canacelCmd;

    public RelayCommand AddCmd { get => _addCmd = (_addCmd = new RelayCommand(AddClick,CanAddClick)); }

    protected abstract void AddClick();

    public RelayCommand CanacelCmd { get => _canacelCmd = (_canacelCmd = new RelayCommand(CancelClick)); }

    public Action<bool> CloseAction { get;internal set; }

    protected virtual void CancelClick()
    {
        CloseAction?.Invoke(false);
    }

    protected bool CanAddClick()
    {
        return true;
    }
}
