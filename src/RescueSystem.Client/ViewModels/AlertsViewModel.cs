using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RescueSystem.Client.Services;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RescueSystem.Client.ViewModels;

public partial class AlertsViewModel : ViewModelBase, IDisposable
{
    private readonly ApiClient _apiClient;
    private readonly SignalRService _signalRService;

    public ObservableCollection<AlertSummaryDto> Alerts { get; } = new();

    [ObservableProperty]
    private bool _isLoading;

    public AlertsViewModel(ApiClient apiClient, SignalRService signalRService)
    {
        _apiClient = apiClient;
        _signalRService = signalRService;

        _signalRService.NewAlertReceived += OnNewAlertReceived;

        LoadAlertsCommand.Execute(null);
    }

    private void OnNewAlertReceived(AlertSummaryDto newAlert)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Alerts.Insert(0, newAlert);
        });
    }

    [RelayCommand]
    private async Task LoadAlertsAsync()
    {
        IsLoading = true;
        Alerts.Clear();

        var queryParams = new PaginationQueryParameters
        {
            PageNumber = 1,
            PageSize = 50
        };

        var pagedResult = await _apiClient.GetAllAlertsAsync(queryParams);

        if (pagedResult != null)
        {
            foreach (var alert in pagedResult.Items)
            {
                Alerts.Add(alert);
            }
        }

        IsLoading = false;
    }

    public void Dispose()
    {
        _signalRService.NewAlertReceived -= OnNewAlertReceived;
    }
}