using IO.Milvus.Workbench.Utils;
using System.Windows;

namespace IO.Milvus.Workbench.ViewModels;

public class AddMilvusDailogViewModel : DialogViewModel
{
    public string Name { get; set; } = "Test";

    public string Host { get; set; } = "192.168.100.139";

    public int Port { get; set; } = 19530;

    public string Username { get; set; }

    public string Password { get; set; }

    protected override void CancelClick()
    {
        CloseAction?.Invoke(false);
    }

    protected override void AddClick()
    {
        if (!PortValidationUtils.PortInRange(Port))
        {
            MessageBox.Show("Port Error");
            return;
        }

        CloseAction?.Invoke(true);
    }
}
