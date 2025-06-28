using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RescueSystem.Client.Services;

namespace RescueSystem.Client.ViewModels;

public partial class ConfigurationViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string _serverUrl;

    [ObservableProperty]
    private bool _savePassword;

    [ObservableProperty]
    private string _statusMessage;

    public ConfigurationViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;

        _serverUrl = _settingsService.CurrentSettings.ServerUrl;
        _savePassword = _settingsService.CurrentSettings.SavePassword;
        _statusMessage = string.Empty;
    }

    [RelayCommand]
    private void SaveSettings()
    {
        _settingsService.CurrentSettings.ServerUrl = ServerUrl;
        _settingsService.CurrentSettings.SavePassword = SavePassword;

        _settingsService.SaveSettings();

        StatusMessage = "Настройки сохранены! Перезапустите приложение для применения.";
    }
}