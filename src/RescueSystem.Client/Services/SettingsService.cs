using System;
using System.IO;
using System.Text.Json;

namespace RescueSystem.Client.Services;

public class SettingsService
{
    private readonly string _settingsFilePath;
    public AppSettings CurrentSettings { get; private set; }

    public SettingsService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "RescueSystemClient");
        Directory.CreateDirectory(appFolder);
        _settingsFilePath = Path.Combine(appFolder, "settings.json");

        CurrentSettings = LoadSettings();
    }

    private AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (settings != null)
                {
                    if (!Uri.TryCreate(settings.ServerUrl, UriKind.Absolute, out _))
                    {
                        Console.WriteLine($"Invalid ServerUrl: {settings.ServerUrl}, using default");
                        settings.ServerUrl = "http://127.0.0.1:8080";
                    }
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }
        return new AppSettings();
    }

    public void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(CurrentSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }
}