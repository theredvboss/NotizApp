using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;


namespace NotizApp;

public interface IDialogService
{
    Task<bool> ShowCloseConfirmationAsync(Window owner);
}

public partial class MainWindow : Window
{
    private readonly IDialogService _dialogService;
    private ProfileService _profileService;
    
    public MainWindow()
    {
        InitializeComponent();
        
        _profileService = new ProfileService();
        //test
        //Console.WriteLine(_profileService.GetProfileFilePath("Victor"));
        this.Loaded += OnLoaded;
        
        _dialogService = new DialogService();
        this.Closing += OnClosing;
    }

    private async void StartButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Debug.Write("Hallo aus dem Log");
        LabelName.IsVisible = true;
        
        await Task.Delay(3000);
        
        ClearScreen();
        
        await Task.Delay(1000);
        
        CreateNote.IsVisible = true;
        OpenNotes.IsVisible = true;
        CloseNotes.IsVisible = true;
    }

    private void CreateNote_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearScreen();
        CreateNoteGrid.IsVisible = true;
        //Hier kommt die elemente zum Erstellen von Notizen
    }

    private void OpenNotes_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearScreen();
        //Hier kommt der Screen mit der Liste von bereits bestehenden Notizen
        //TIPP: Notizen speichern (aber wie??)
    }

    private void CloseNotes_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ClearScreen()
    {
        LabelName.IsVisible = false;
        StartButton.IsVisible = false;
        CreateNote.IsVisible = false;
        OpenNotes.IsVisible = false;
        CloseNotes.IsVisible = false;
        CreateNoteGrid.IsVisible = false;
    }
    
    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    { 
        e.Cancel = true;
        var dialog = new DialogJaNein();
        bool result = await dialog.ShowDialogAsync(this);

        if (result)
        {
            this.Closing -= OnClosing;
            this.Close();
        }
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearScreen();
        CreateNote.IsVisible = true;
        OpenNotes.IsVisible = true;
        CloseNotes.IsVisible = true;
    }
    private async void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // 1. Neues Profil speichern
        var newProfile = new Profile("Victor", new List<Note>());
        await _profileService.SaveProfileAsync(newProfile);
        Console.WriteLine("Profil gespeichert!");
        
        
        // 2. Profil laden
        var loadedProfile = await _profileService.LoadProfileAsync("Sergiu");
        if (loadedProfile != null)
            Console.WriteLine($"Profil geladen: {loadedProfile.ProfileName}");
        else
            Console.WriteLine("Profil konnte nicht geladen werden.");

       
        var allProfiles = _profileService.GetAllProfiles();
        foreach (var name in allProfiles)
        {
            Console.WriteLine("Gefundenes Profil: " + name);
        }
    }
}