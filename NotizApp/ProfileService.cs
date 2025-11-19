using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotizApp;

public class ProfileService
{
    
    public string ProfilesFolderPath { get; }

    public ProfileService()
    {
        //universeller pfad auf allen Betriebsystemen
        ProfilesFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NotizApp",
            "Profiles");

        // Ordner erstellen, falls er nicht existiert
        if (!Directory.Exists(ProfilesFolderPath))
        {
            Directory.CreateDirectory(ProfilesFolderPath);
        }
    }
    
    public string GetProfileFilePath(string profileName)
    {
        return Path.Combine(ProfilesFolderPath, profileName + ".json");
    }
    
    public async Task SaveProfileAsync(Profile profile)
    {
        // 1. Pfad erzeugen
        string filePath = GetProfileFilePath(profile.ProfileName);

        // 2. Objekt → JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // schön formatiert
        };

        string json = JsonSerializer.Serialize(profile, options);

        // 3. JSON in Datei schreiben (asynchron)
        await File.WriteAllTextAsync(filePath, json);
    }
    
    public async Task<Profile?> LoadProfileAsync(string profileName)
    {
        string filePath = GetProfileFilePath(profileName);

        // 1. Datei existiert nicht → null zurückgeben
        if (!File.Exists(filePath))
            return null;

        try
        {
            Console.WriteLine($"{profileName}'s Profil wird geladen");
            // 2. Dateiinhalt lesen
            string json = await File.ReadAllTextAsync(filePath);

            // 3. JSON → Profile Objekt
            var profile = JsonSerializer.Deserialize<Profile>(json);

            return profile;
        }
        catch
        {
            // Datei existiert, aber ist beschädigt → null zurückgeben
            return null;
        }
    }

    public List<string> GetAllProfiles()
    {
        string[] files = Directory.GetFiles(ProfilesFolderPath, "*.json"); // ein array mit allen profile namen

        List<string> profileNames = new List<string>();
        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file); //entfernt .json 
            profileNames.Add(fileName); //fügt nur den profilnamen in der Liste hinzu
        }

        return profileNames;
    }
}