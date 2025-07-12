using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RescueSystem.Client.Services;
using RescueSystem.Client.ViewModels;
using RescueSystem.Client.Views;
using System;

namespace RescueSystem.Client
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };

                var signalRService = serviceProvider.GetRequiredService<SignalRService>();

                _ = signalRService.StartConnectionAsync();

                desktop.ShutdownRequested += async (sender, args) =>
                {
                    var signalRToDispose = serviceProvider.GetService<SignalRService>();
                    if (signalRToDispose != null)
                    {
                        await signalRToDispose.DisposeAsync();
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SettingsService>();
            services.AddHttpClient<ApiClient>((serviceProvider, client) =>
            {
                var settingsService = serviceProvider.GetRequiredService<SettingsService>();
                var serverUrl = settingsService.CurrentSettings.ServerUrl;

                try
                {
                    client.BaseAddress = new Uri(serverUrl);
                }
                catch (UriFormatException)
                {
                    client.BaseAddress = new Uri("https://localhost:7043");
                    Console.WriteLine($"Invalid ServerUrl, using fallback");
                }
            });
            services.AddSingleton<SignalRService>(serviceProvider =>
            {
                var settingsService = serviceProvider.GetRequiredService<SettingsService>();
                return new SignalRService(settingsService);
            });
            services.AddSingleton<IInteractionService, DialogInteractionService>();
            services.AddTransient<AlertsViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<BraceletsViewModel>();

            services.AddTransient<Func<AlertsViewModel>>(sp => () => sp.GetRequiredService<AlertsViewModel>());
            services.AddTransient<Func<UsersViewModel>>(sp => () => sp.GetRequiredService<UsersViewModel>());
            services.AddTransient<Func<BraceletsViewModel>>(sp => () => sp.GetRequiredService<BraceletsViewModel>());
            services.AddTransient<ConfigurationViewModel>();
            services.AddTransient<Func<ConfigurationViewModel>>(sp => () => sp.GetRequiredService<ConfigurationViewModel>());
            services.AddSingleton<MainViewModel>();
        }
    }
}