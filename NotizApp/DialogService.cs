using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace NotizApp;

public class DialogService : IDialogService
{
    public async Task<bool> ShowCloseConfirmationAsync(Window owner)
    {
        var dialog = new DialogJaNein();
        bool result = await dialog.ShowDialogAsync(owner);
        return result;
    }
}