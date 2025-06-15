using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;
using ScheduleEditor.ViewModels.Base;

namespace ScheduleEditor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IProcessService _processService;
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;

        private object? _currentView;
        private Process? _selectedProcess;
        private Schedule? _selectedSchedule;

        public MainViewModel(
            IProcessService processService,
            IDialogService dialogService,
            IFileService fileService)
        {
            _processService = processService;
            _dialogService = dialogService;
            _fileService = fileService;

            ShowProcessListCommand = new RelayCommand(ShowProcessList);
            ExportToExcelCommand = new RelayCommand(async () => await ExportToExcelAsync(), () => SelectedSchedule != null);
            ExitCommand = new RelayCommand(() => System.Windows.Application.Current.Shutdown());
            ShowSettingsCommand = new RelayCommand(ShowSettings);

            // 시작 시 프로세스 목록 표시
            ShowProcessList();
        }

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public Process? SelectedProcess
        {
            get => _selectedProcess;
            set => SetProperty(ref _selectedProcess, value);
        }

        public Schedule? SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }

        public ICommand ShowProcessListCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        private void ShowProcessList()
        {
            CurrentView = new ProcessListViewModel(_processService, _dialogService, this);
        }

        private async Task ExportToExcelAsync()
        {
            if (SelectedSchedule == null) return;

            var fileName = await _fileService.SaveFileAsync(
                "Excel Files (*.xlsx)|*.xlsx",
                $"{SelectedSchedule.Name}.xlsx");

            if (!string.IsNullOrEmpty(fileName))
            {
                await _fileService.ExportToExcelAsync(SelectedSchedule, fileName);
                await _dialogService.ShowInfoAsync($"Excel 파일로 내보내기 완료: {fileName}");
            }
        }

        private void ShowSettings()
        {
            CurrentView = new SettingsViewModel();
        }

        public void ShowScheduleList(Process process)
        {
            SelectedProcess = process;
            CurrentView = new ScheduleListViewModel(_processService, _dialogService, _fileService, this, process);
        }

        public void ShowScheduleEdit(Schedule schedule)
        {
            SelectedSchedule = schedule;
            CurrentView = new ScheduleEditViewModel(_dialogService, this, schedule);
        }
    }
}