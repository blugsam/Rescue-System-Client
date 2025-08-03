namespace RescueSystem.Client.Services;

public class AppSettings
{
    public string ServerUrl { get; set; } = "http://127.0.0.1:8080";
    public bool SavePassword { get; set; } = false;
}