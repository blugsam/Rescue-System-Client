using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RescueSystem.Client.Services;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RescueSystem.Client.ViewModels;

public partial class BraceletsViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    private readonly IInteractionService _interactionService;

    public ObservableCollection<BraceletDto> Bracelets { get; } = new();
    public ObservableCollection<UserSummaryDto> AllUsers { get; } = new();

    public List<BraceletStatus> AllStatuses { get; } = Enum.GetValues<BraceletStatus>().ToList();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteBraceletCommand))]
    [NotifyCanExecuteChangedFor(nameof(AssignUserCommand))]
    [NotifyCanExecuteChangedFor(nameof(UnassignUserCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveStatusCommand))]
    private BraceletDetailsDto? _selectedBraceletDetails;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AssignUserCommand))]
    private UserSummaryDto? _selectedUserToAssign;
    private BraceletDto? _selectedBraceletInList;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveStatusCommand))]
    private BraceletStatus? _selectedStatus;

    public BraceletDto? SelectedBraceletInList
    {
        get => _selectedBraceletInList;
        set
        {
            if (SetProperty(ref _selectedBraceletInList, value) && value != null)
            {
                LoadBraceletDetailsCommand.Execute(value.Id);
            }
        }
    }

    public BraceletsViewModel(ApiClient apiClient, IInteractionService interactionService)
    {
        _apiClient = apiClient;
        _interactionService = interactionService;

        LoadBraceletsCommand.Execute(null);
        LoadAllUsersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadBraceletsAsync()
    {
        IsLoading = true;
        Bracelets.Clear();
        SelectedBraceletDetails = null;
        SelectedBraceletInList = null;

        var queryParams = new PaginationQueryParameters { PageSize = 100 };
        var pagedResult = await _apiClient.GetAllBraceletsAsync(queryParams);
        if (pagedResult != null)
        {
            foreach (var bracelet in pagedResult.Items)
            {
                Bracelets.Add(bracelet);
            }
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadAllUsersAsync()
    {
        AllUsers.Clear();
        var queryParams = new PaginationQueryParameters { PageSize = 1000 };
        var pagedResult = await _apiClient.GetAllUsersAsync(queryParams);
        if (pagedResult != null)
        {
            foreach (var user in pagedResult.Items)
            {
                AllUsers.Add(user);
            }
        }
    }

    [RelayCommand]
    private async Task LoadBraceletDetailsAsync(Guid braceletId)
    {
        SelectedBraceletDetails = await _apiClient.GetBraceletByIdAsync(braceletId);
        if (SelectedBraceletDetails != null)
        {
            if (Enum.TryParse<BraceletStatus>(SelectedBraceletDetails.Status, true, out var parsedStatus))
            {
                SelectedStatus = parsedStatus;
            }
            else
            {
                SelectedStatus = null;
            }
        }
    }

    [RelayCommand]
    private async Task CreateBraceletAsync()
    {
        var serialNumber = await _interactionService.ShowInputDialogAsync("Новый браслет", "Введите серийный номер:");

        if (string.IsNullOrWhiteSpace(serialNumber))
            return;

        try
        {
            var request = new CreateBraceletRequest { SerialNumber = serialNumber };
            await _apiClient.CreateBraceletAsync(request);
            await LoadBraceletsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create bracelet: {ex.Message}");
            await _interactionService.ShowMessageAsync("Ошибка", $"Не удалось создать браслет: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteBracelet))]
    private async Task DeleteBraceletAsync()
    {
        if (SelectedBraceletDetails is null) return;

        try
        {
            await _apiClient.DeleteBraceletAsync(SelectedBraceletDetails.Id);
            await LoadBraceletsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to delete bracelet: {ex.Message}");
            await _interactionService.ShowMessageAsync("Ошибка", $"Не удалось удалить браслет: {ex.Message}");
        }
    }

    private bool CanDeleteBracelet()
    {
        return SelectedBraceletDetails != null && SelectedBraceletDetails.AssignedUser == null;
    }

    [RelayCommand(CanExecute = nameof(CanSaveStatus))]
    private async Task SaveStatusAsync()
    {
        if (SelectedBraceletDetails is null || SelectedStatus is null) return;

        var braceletIdToUpdate = SelectedBraceletDetails.Id;
        var newStatusEnum = SelectedStatus.Value;

        try
        {
            var request = new UpdateBraceletRequest { Status = newStatusEnum.ToString() };
            await _apiClient.UpdateBraceletStatusAsync(braceletIdToUpdate, request);

            await LoadBraceletsAsync();

            SelectedBraceletInList = Bracelets.FirstOrDefault(b => b.Id == braceletIdToUpdate);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save status: {ex.Message}");
            await _interactionService.ShowMessageAsync("Ошибка", "Не удалось сохранить статус.");
        }
    }

    [RelayCommand(CanExecute = nameof(CanAssignUser))]
    private async Task AssignUserAsync()
    {
        if (SelectedBraceletDetails is null || SelectedUserToAssign is null) return;

        try
        {
            await _apiClient.AssignUserToBraceletAsync(SelectedBraceletDetails.Id, SelectedUserToAssign.Id);
            await LoadBraceletDetailsAsync(SelectedBraceletDetails.Id);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to assign user: {ex.Message}");
        }
    }

    private bool CanAssignUser() => SelectedBraceletDetails?.AssignedUser is null && SelectedUserToAssign is not null;

    [RelayCommand(CanExecute = nameof(CanUnassignUser))]
    private async Task UnassignUserAsync()
    {
        if (SelectedBraceletDetails is null) return;

        try
        {
            await _apiClient.UnassignUserFromBraceletAsync(SelectedBraceletDetails.Id);
            await LoadBraceletDetailsAsync(SelectedBraceletDetails.Id);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to unassign user: {ex.Message}");
        }
    }
    private bool CanUnassignUser() => SelectedBraceletDetails?.AssignedUser is not null;

    private bool CanSaveStatus()
    {
        if (SelectedBraceletDetails is null || SelectedStatus is null)
        {
            return false;
        }

        return SelectedBraceletDetails.Status != SelectedStatus.Value.ToString();
    }
}