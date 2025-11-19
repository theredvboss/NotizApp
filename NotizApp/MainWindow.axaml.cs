using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;


namespace NotizApp;

public interface IDialogService
{
    Task<bool> ShowCloseConfirmationAsync(Window owner);
}

public partial class MainWindow : Window
{
    private readonly IDialogService _dialogService;
    private ProfileService _profileService;
    private Profile? _currentProfile;
    
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
    

    private void CreateNote_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearScreen();
        CreateNoteGrid.IsVisible = true;
        
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
        CreateNote.IsVisible = false;
        OpenNotes.IsVisible = false;
        SwitchUser.IsVisible = false;
        CloseNotes.IsVisible = false;
        CreateNoteGrid.IsVisible = false;
        ChooseProfile.IsVisible = false;
        CreateNewProfile.IsVisible = false;
        ProfileListPanel.IsVisible = false;
        ProfileCreationPanel.IsVisible = false;
    }

    private void BackToMenu()
    {
        ClearScreen();
        CreateNote.IsVisible = true;
        OpenNotes.IsVisible = true;
        SwitchUser.IsVisible = true;
        CloseNotes.IsVisible = true;
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
        BackToMenu();
    }
    private async void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
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

    private void ChooseProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearScreen();

        CreateNewProfile.IsVisible = true;
        ProfileListPanel.IsVisible = true;

        var profiles = _profileService.GetAllProfiles();

        if (profiles.Count == 0)
        {
            Console.WriteLine("Kein Profil gefunden!");
            CreateNewProfile.Margin = new Thickness(0,0,0,0);
            LabelName.IsVisible = true;
            LabelName.Content = "Bitte erstelle ein neues Profil.";
            return;
        }

        // Wenn es Profile gibt
        Console.WriteLine("Es gibt vorhandene Profile");
        ProfileListPanel.Children.Clear();
        LabelName.IsVisible = true;
        LabelName.Content = "Wähle ein Profil aus:";

        foreach (var profile in profiles)
        {
            var b = new Button
            {
                Content = profile,
                Width = 200,
                Height = 40,
                FontSize = 20,
                Margin = new Thickness(5),
                Background = Brushes.LightGoldenrodYellow
            };

            b.Click += async (s, e) =>
            {
                _currentProfile = await _profileService.LoadProfileAsync(profile);
                AccessMenu();
            };

            ProfileListPanel.Children.Add(b);
        }
    }

    private void CreateNewProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearScreen();
        ProfileCreationPanel.IsVisible = true;
    }

    private void SaveNewProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        
        _currentProfile = new Profile(NewProfileNameBox.Text, new List<Note>());
        _profileService.SaveProfileAsync(_currentProfile);
        AccessMenu();
    }

    private async void AccessMenu()
    {
        ClearScreen();
        LabelName.IsVisible = true;
        LabelName.Content = $"Willkommen {_currentProfile.ProfileName}, in deiner NotizApp!";
        await Task.Delay(2000);
        BackToMenu();
    }

    private async void SwitchUser_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_currentProfile != null)
        {
            await _profileService.SaveProfileAsync(_currentProfile);
        }

        _currentProfile = null;
        ClearScreen();
        ChooseProfile_OnClick(sender, e);
    }

    private async void SaveNoteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string title = TitelBox.Text;
        string content = InhaltsBox.Text;

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Titel darf nicht leer sein!");
            return;
        }

        Note newNote = new Note(title, content);
        _currentProfile.Notes.Add(newNote);
        await _profileService.SaveProfileAsync(_currentProfile);

        Console.WriteLine($"Notiz '{title}' gespeichert!");
        await Task.Delay(3000);
        BackToMenu();
    }
}