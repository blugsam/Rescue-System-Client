using RescueSystem.Contracts.Contracts.Models;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace RescueSystem.Client.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    #region Alerts API

    public async Task<PagedResult<AlertSummaryDto>?> GetAllAlertsAsync(PaginationQueryParameters queryParams)
    {
        var url = $"api/alerts?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
        return await GetAsync<PagedResult<AlertSummaryDto>>(url);
    }

    public async Task<AlertDetailsDto?> GetAlertByIdAsync(Guid id)
    {
        return await GetAsync<AlertDetailsDto>($"api/alerts/{id}");
    }

    public async Task<AlertDetailsDto?> CreateAlertAsync(CreateAlertRequest request)
    {
        return await PostAsync<CreateAlertRequest, AlertDetailsDto>("api/alerts", request);
    }

    public async Task DeleteAlertAsync(Guid id)
    {
        await DeleteAsync($"api/alerts/{id}");
    }

    #endregion

    #region Users API

    public async Task<PagedResult<UserSummaryDto>?> GetAllUsersAsync(PaginationQueryParameters queryParams)
    {
        var url = $"api/users?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}&searchTerm={queryParams.SearchTerm}";
        return await GetAsync<PagedResult<UserSummaryDto>>(url);
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(Guid id)
    {
        return await GetAsync<UserDetailsDto>($"api/users/{id}");
    }

    public async Task<UserDetailsDto?> CreateUserAsync(CreateUserRequest request)
    {
        return await PostAsync<CreateUserRequest, UserDetailsDto>("api/users", request);
    }

    public async Task<UserDetailsDto?> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        return await PutAsync<UpdateUserRequest, UserDetailsDto>($"api/users/{id}", request);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await DeleteAsync($"api/users/{id}");
    }

    #endregion

    #region Bracelets API

    public async Task<PagedResult<BraceletDto>?> GetAllBraceletsAsync(PaginationQueryParameters queryParams)
    {
        var url = $"api/bracelets?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
        return await GetAsync<PagedResult<BraceletDto>>(url);
    }

    public async Task<BraceletDetailsDto?> GetBraceletByIdAsync(Guid id)
    {
        return await GetAsync<BraceletDetailsDto>($"api/bracelets/{id}");
    }

    public async Task<BraceletDetailsDto?> CreateBraceletAsync(CreateBraceletRequest request)
    {
        return await PostAsync<CreateBraceletRequest, BraceletDetailsDto>("api/bracelets", request);
    }

    public async Task UpdateBraceletStatusAsync(Guid id, UpdateBraceletRequest request)
    {
        await PutAsync($"api/bracelets/{id}/status", request);
    }

    public async Task AssignUserToBraceletAsync(Guid braceletId, Guid userId)
    {
        var request = new AssignUserToBraceletRequest { UserId = userId };
        await PostAsync($"api/bracelets/{braceletId}/assignment", request);
    }

    public async Task UnassignUserFromBraceletAsync(Guid braceletId)
    {
        await DeleteAsync($"api/bracelets/{braceletId}/assignment");
    }

    public async Task DeleteBraceletAsync(Guid id)
    {
        await DeleteAsync($"api/bracelets/{id}");
    }

    #endregion

    #region Private helper methods

    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<T>(url, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"API GET request to {url} failed: {ex.StatusCode} - {ex.Message}");

            return null;
        }
    }

    private async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data);
        return await HandleResponse<TResponse>(response);
    }

    private async Task PostAsync<TRequest>(string url, TRequest data)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data);
        await HandleResponse(response);
    }

    private async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await _httpClient.PutAsJsonAsync(url, data);
        return await HandleResponse<TResponse>(response);
    }

    private async Task PutAsync<TRequest>(string url, TRequest data)
    {
        var response = await _httpClient.PutAsJsonAsync(url, data);
        await HandleResponse(response);
    }

    private async Task DeleteAsync(string url)
    {
        var response = await _httpClient.DeleteAsync(url);
        await HandleResponse(response);
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API request failed: {response.StatusCode}. Content: {errorContent}");
            response.EnsureSuccessStatusCode();
        }
    }

    private async Task<T?> HandleResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentLength == 0) return default;
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        await HandleResponse(response);
        return default;
    }

    #endregion
}