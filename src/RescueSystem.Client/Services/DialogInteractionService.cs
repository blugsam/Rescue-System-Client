using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RescueSystem.Client.Services;

public class DialogInteractionService : IInteractionService
{
    public Task ShowMessageAsync(string title, string message)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            new MessageBoxStandardParams
            {
                ContentTitle = title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.Ok,
                Icon = Icon.Info,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            });

        return Dispatcher.UIThread.InvokeAsync(() => messageBox.ShowAsync());
    }

    public async Task<string?> ShowInputDialogAsync(string title, string message)
    {
        var customParams = new MessageBoxCustomParams
        {
            ContentTitle = title,
            ContentMessage = message,
            InputParams = new InputParams
            {
                DefaultValue = ""
            },
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ButtonDefinitions = new List<ButtonDefinition>
            {
                new ButtonDefinition { Name = "OK", IsDefault = true },
                new ButtonDefinition { Name = "Cancel", IsCancel  = true }
            },
            Icon = Icon.Question,
            CanResize = false
        };

        var messageBoxWindow = MessageBoxManager.GetMessageBoxCustom(customParams);

        string? result = null;

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var ownerWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (ownerWindow != null)
            {
                result = await messageBoxWindow.ShowWindowDialogAsync(ownerWindow);
            }
            else
            {
                result = await messageBoxWindow.ShowAsync();
            }
        });

        if (result == "OK" && !string.IsNullOrWhiteSpace(messageBoxWindow.InputValue))
        {
            return messageBoxWindow.InputValue.Trim();
        }

        return null;
    }
}