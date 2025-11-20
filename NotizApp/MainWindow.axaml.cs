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
    #region Felder
    private readonly IDialogService _dialogService;
    private ProfileService _profileService;
    private Profile? _currentProfile;
    private List<Control> _screens;

    #endregion

    #region Kontruktor
    public MainWindow()
    {
        InitializeComponent();
        
        _profileService = new ProfileService();
        _dialogService = new DialogService();
        
        _screens = new List<Control>
        {
            ChooseProfileGrid,
            ProfileCreationGrid,
            MenuGrid,
            CreateNoteGrid
        };
        
        this.Loaded += OnLoaded;
        this.Closing += OnClosing;
    }
    #endregion

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
        foreach (var screen in _screens)
            screen.IsVisible = false;

        LabelGrid.IsVisible = false;
        ChooseProfile.IsVisible = false;
    }

    private void BackToMenu()
    {
        ClearScreen();
        MenuGrid.IsVisible = true;
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

        ChooseProfileGrid.IsVisible = true;
        LabelGrid.IsVisible = true;

        var profiles = _profileService.GetAllProfiles();

        if (profiles.Count == 0)
        {
            CreateNewProfile.Margin = new Thickness(0,0,0,0);
            LabelName.Content = "Bitte erstelle ein neues Profil.";
            return;
        }

        // Wenn es Profile gibt
        ProfileListPanel.Children.Clear();
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
                Foreground = Brushes.Black
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
        ProfileCreationGrid.IsVisible = true;
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
        LabelGrid.IsVisible = true;
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
            //titel darf nicht leer sein
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