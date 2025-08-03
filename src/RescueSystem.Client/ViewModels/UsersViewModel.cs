using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RescueSystem.Client.Services;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RescueSystem.Client.ViewModels;

public partial class UsersViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    public ObservableCollection<UserSummaryDto> Users { get; } = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteUserCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveUserCommand))]
    private UserDetailsDto? _selectedUserDetails;

    private UserSummaryDto? _selectedUserInList;
    public UserSummaryDto? SelectedUserInList
    {
        get => _selectedUserInList;
        set
        {
            if (SetProperty(ref _selectedUserInList, value) && value != null)
            {
                _isNewUserMode = false;
                LoadUserDetailsCommand.Execute(value.Id);
            }
        }
    }

    private bool _isNewUserMode = false;

    public UsersViewModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
        LoadUsersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        Users.Clear();
        SelectedUserDetails = null;
        SelectedUserInList = null;
        _isNewUserMode = false;

        var queryParams = new PaginationQueryParameters { PageSize = 100 };
        var pagedResult = await _apiClient.GetAllUsersAsync(queryParams);
        if (pagedResult != null)
        {
            foreach (var user in pagedResult.Items)
            {
                Users.Add(user);
            }
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadUserDetailsAsync(Guid userId)
    {
        SelectedUserDetails = await _apiClient.GetUserByIdAsync(userId);
    }

    [RelayCommand]
    private void AddNewUser()
    {
        _isNewUserMode = true;

        SelectedUserInList = null;

        SelectedUserDetails = new UserDetailsDto
        {
            Id = Guid.Empty, 
            FullName = "",
            DateOfBirth = new DateOnly(DateTime.Now.Year - 20, 1, 1),
            MedicalNotes = "",
            EmergencyContact = ""
        };
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrSaveUser))]
    private async Task DeleteUserAsync()
    {
        if (SelectedUserDetails is null || _isNewUserMode) return;

        try
        {
            await _apiClient.DeleteUserAsync(SelectedUserDetails.Id);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to delete user: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrSaveUser))]
    private async Task SaveUserAsync()
    {
        if (SelectedUserDetails is null) return;

        try
        {
            if (_isNewUserMode)
            {
                var createRequest = new CreateUserRequestDto
                {
                    FullName = SelectedUserDetails.FullName,
                    DateOfBirth = SelectedUserDetails.DateOfBirth,
                    MedicalNotes = SelectedUserDetails.MedicalNotes,
                    EmergencyContact = SelectedUserDetails.EmergencyContact
                };
                await _apiClient.CreateUserAsync(createRequest);
            }
            else
            {
                var updateRequest = new UpdateUserRequestDto
                {
                    FullName = SelectedUserDetails.FullName,
                    DateOfBirth = SelectedUserDetails.DateOfBirth,
                    MedicalNotes = SelectedUserDetails.MedicalNotes,
                    EmergencyContact = SelectedUserDetails.EmergencyContact
                };
                await _apiClient.UpdateUserAsync(SelectedUserDetails.Id, updateRequest);
            }

            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save user: {ex.Message}");
        }
    }

    private bool CanDeleteOrSaveUser() => SelectedUserDetails != null;
}