using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RescueSystem.Client.Services;
using System;

namespace RescueSystem.Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Func<AlertsViewModel> _alertsViewModelFactory;
    private readonly Func<UsersViewModel> _usersViewModelFactory;
    private readonly Func<BraceletsViewModel> _braceletsViewModelFactory;
    private readonly Func<ConfigurationViewModel> _configViewModelFactory;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    public bool IsAlertsPageActive => CurrentPage is AlertsViewModel;
    public bool IsUsersPageActive => CurrentPage is UsersViewModel;
    public bool IsBraceletsPageActive => CurrentPage is BraceletsViewModel;
    public bool IsConfigurationPageActive => CurrentPage is ConfigurationViewModel;


    public MainViewModel(
        Func<AlertsViewModel> alertsViewModelFactory,
        Func<UsersViewModel> usersViewModelFactory,
        Func<BraceletsViewModel> braceletsViewModelFactory,
        Func<ConfigurationViewModel> configViewModelFactory)

    {
        _alertsViewModelFactory = alertsViewModelFactory;
        _usersViewModelFactory = usersViewModelFactory;
        _braceletsViewModelFactory = braceletsViewModelFactory;
        _configViewModelFactory = configViewModelFactory;

        _currentPage = _alertsViewModelFactory();
    }

    partial void OnCurrentPageChanged(ViewModelBase value)
    {
        OnPropertyChanged(nameof(IsAlertsPageActive));
        OnPropertyChanged(nameof(IsUsersPageActive));
        OnPropertyChanged(nameof(IsBraceletsPageActive));
        OnPropertyChanged(nameof(IsConfigurationPageActive));
    }

    [RelayCommand]
    private void NavigateToAlerts() => CurrentPage = _alertsViewModelFactory();

    [RelayCommand]
    private void NavigateToUsers() => CurrentPage = _usersViewModelFactory();

    [RelayCommand]
    private void NavigateToBracelets() => CurrentPage = _braceletsViewModelFactory();
    [RelayCommand]
    private void NavigateToConfiguration() => CurrentPage = _configViewModelFactory();
}