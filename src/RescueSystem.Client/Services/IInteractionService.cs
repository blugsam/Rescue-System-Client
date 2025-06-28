using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RescueSystem.Client.Services;

public interface IInteractionService
{
    Task ShowMessageAsync(string title, string message);
    Task<string?> ShowInputDialogAsync(string title, string message);
    //Task<T?> ShowSelectionDialogAsync<T>(string title, string message, IEnumerable<T> options) where T : Enum;
}