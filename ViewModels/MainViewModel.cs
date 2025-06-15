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

            // Commands 초기화
            ShowProcessListCommand = new RelayCommand(ShowProcessList);
            ExportToExcelCommand = new RelayCommand(async () => await ExportToExcelAsync(), () => SelectedSchedule != null);
            ExitCommand = new RelayCommand(() => System.Windows.Application.Current.Shutdown());
            ShowSettingsCommand = new RelayCommand(ShowSettings);

            // 시작 시 프로세스 목록 표시
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                // 서비스 초기화 확인
                await _processService.GetAllProcessesAsync();
                ShowProcessList();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync($"초기화 중 오류가 발생했습니다: {ex.Message}");
                ShowProcessList(); // 오류가 있어도 기본 화면은 표시
            }
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
            try
            {
                CurrentView = new ProcessListViewModel(_processService, _dialogService, this);
            }
            catch (System.Exception ex)
            {
                _dialogService.ShowErrorAsync($"프로세스 목록을 표시하는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private async Task ExportToExcelAsync()
        {
            if (SelectedSchedule == null) return;

            try
            {
                var fileName = await _fileService.SaveFileAsync(
                    "Excel Files (*.xlsx)|*.xlsx",
                    $"{SelectedSchedule.Name}.xlsx");

                if (!string.IsNullOrEmpty(fileName))
                {
                    await _fileService.ExportToExcelAsync(SelectedSchedule, fileName);
                    await _dialogService.ShowInfoAsync($"Excel 파일로 내보내기 완료: {fileName}");
                }
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync($"Excel 내보내기 중 오류가 발생했습니다: {ex.Message}");
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