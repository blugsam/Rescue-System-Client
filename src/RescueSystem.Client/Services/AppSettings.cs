namespace RescueSystem.Client.Models;

public class AppSettings
{
    public string ServerUrl { get; set; } = "https://localhost:7043";
    public bool SavePassword { get; set; } = false;
}