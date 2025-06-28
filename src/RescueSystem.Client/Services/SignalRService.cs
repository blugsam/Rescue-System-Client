using Microsoft.AspNetCore.SignalR.Client;
using RescueSystem.Contracts.Contracts.Responses;
using System;
using System.Threading.Tasks;

namespace RescueSystem.Client.Services;

public class SignalRService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    public event Action<AlertSummaryDto>? NewAlertReceived;

    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

    public SignalRService(SettingsService settingsService)
    {
        var hubUrl = settingsService.CurrentSettings.ServerUrl.TrimEnd('/') + "/alert-hub";

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<AlertSummaryDto>("NewAlertReceived", (alert) =>
        {
            NewAlertReceived?.Invoke(alert);
        });
    }

    public async Task StartConnectionAsync()
    {
        if (IsConnected) return;

        try
        {
            await _hubConnection.StartAsync();
            Console.WriteLine("SignalR Connection Started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting SignalR connection: {ex.Message}");
        }
    }

    public async Task StopConnectionAsync()
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.StopAsync();
            Console.WriteLine("SignalR Connection Stopped.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopConnectionAsync();
        await _hubConnection.DisposeAsync();
    }
}