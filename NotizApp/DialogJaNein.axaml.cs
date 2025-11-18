using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace NotizApp;

public partial class DialogJaNein : Window
{
    private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

    public DialogJaNein()
    {
        InitializeComponent();

        YesButton.Click += (s, e) => Close(true);
        NoButton.Click += (s, e) => Close(false);
    }

    public Task<bool> ShowDialogAsync(Window owner)
    {
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        // Zeigt das Fenster modal und blockiert bis es geschlossen wird
        return this.ShowDialog<bool>(owner);
    }
}