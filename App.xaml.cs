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

            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // MainWindow 생성 및 DataContext 설정
                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                var mainWindow = new MainWindow();

                // DataContext 설정 후 Show 호출
                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();

                // 메인 윈도우 설정
                this.MainWindow = mainWindow;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"애플리케이션 시작 중 오류가 발생했습니다: {ex.Message}",
                               "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Services (Singleton으로 등록)
            services.AddSingleton<IProcessService, ProcessService>();
            services.AddSingleton<IScheduleService, ScheduleService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModels (Singleton으로 변경)
            services.AddSingleton<MainViewModel>();
            services.AddTransient<ProcessListViewModel>();
            services.AddTransient<ScheduleListViewModel>();
            services.AddTransient<ScheduleEditViewModel>();
            services.AddTransient<SettingsViewModel>();
        }
    }
}