using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScheduleEditor.Services;
using ScheduleEditor.Services.Interfaces;
using ScheduleEditor.ViewModels;

namespace ScheduleEditor
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton<IScheduleService, ScheduleService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ProcessListViewModel>();
            services.AddTransient<ScheduleListViewModel>();
            services.AddTransient<ScheduleEditViewModel>();
            services.AddTransient<SettingsViewModel>();
        }
    }
}