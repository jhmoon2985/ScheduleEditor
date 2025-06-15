using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;
using ScheduleEditor.ViewModels.Base;

namespace ScheduleEditor.ViewModels
{
    public class ScheduleListViewModel : ViewModelBase
    {
        private readonly IProcessService _processService;
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;
        private readonly MainViewModel _mainViewModel;
        private readonly Process _process;

        private Schedule? _selectedSchedule;

        public ScheduleListViewModel(
            IProcessService processService,
            IDialogService dialogService,
            IFileService fileService,
            MainViewModel mainViewModel,
            Process process)
        {
            _processService = processService;
            _dialogService = dialogService;
            _fileService = fileService;
            _mainViewModel = mainViewModel;
            _process = process;

            Schedules = new ObservableCollection<Schedule>();

            AddScheduleCommand = new RelayCommand(async () => await AddScheduleAsync());
            EditScheduleCommand = new RelayCommand(async () => await EditScheduleAsync(), () => SelectedSchedule != null);
            DeleteScheduleCommand = new RelayCommand(async () => await DeleteScheduleAsync(), () => SelectedSchedule != null);
            CopyScheduleCommand = new RelayCommand(async () => await CopyScheduleAsync(), () => SelectedSchedule != null);
            ImportScheduleCommand = new RelayCommand(async () => await ImportScheduleAsync());
            ExportScheduleCommand = new RelayCommand(async () => await ExportScheduleAsync(), () => SelectedSchedule != null);
            SelectScheduleCommand = new RelayCommand(SelectSchedule, () => SelectedSchedule != null);

            LoadSchedules();
        }

        public ObservableCollection<Schedule> Schedules { get; }
        public string ProcessName => _process.Name;

        public Schedule? SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }

        public ICommand AddScheduleCommand { get; }
        public ICommand EditScheduleCommand { get; }
        public ICommand DeleteScheduleCommand { get; }
        public ICommand CopyScheduleCommand { get; }
        public ICommand ImportScheduleCommand { get; }
        public ICommand ExportScheduleCommand { get; }
        public ICommand SelectScheduleCommand { get; }

        private void LoadSchedules()
        {
            Schedules.Clear();
            foreach (var schedule in _process.Schedules)
            {
                Schedules.Add(schedule);
            }
        }

        private async Task AddScheduleAsync()
        {
            var name = await _dialogService.ShowInputDialogAsync("스케줄 등록", "스케줄명을 입력하세요:");
            if (string.IsNullOrEmpty(name)) return;

            var description = await _dialogService.ShowInputDialogAsync("스케줄 등록", "설명을 입력하세요:");
            var author = await _dialogService.ShowInputDialogAsync("스케줄 등록", "작성자를 입력하세요:");

            var schedule = new Schedule
            {
                Name = name,
                Description = description ?? "",
                Author = author ?? ""
            };

            _process.Schedules.Add(schedule);
            Schedules.Add(schedule);
            await _processService.UpdateProcessAsync(_process);
        }

        private async Task EditScheduleAsync()
        {
            if (SelectedSchedule == null) return;

            var name = await _dialogService.ShowInputDialogAsync("스케줄 수정", "스케줄명:", SelectedSchedule.Name);
            if (string.IsNullOrEmpty(name)) return;

            var description = await _dialogService.ShowInputDialogAsync("스케줄 수정", "설명:", SelectedSchedule.Description);
            var author = await _dialogService.ShowInputDialogAsync("스케줄 수정", "작성자:", SelectedSchedule.Author);

            SelectedSchedule.Name = name;
            SelectedSchedule.Description = description ?? "";
            SelectedSchedule.Author = author ?? "";

            await _processService.UpdateProcessAsync(_process);
        }

        private async Task DeleteScheduleAsync()
        {
            if (SelectedSchedule == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "스케줄 삭제",
                $"'{SelectedSchedule.Name}' 스케줄을 삭제하시겠습니까?");

            if (confirmed)
            {
                _process.Schedules.Remove(SelectedSchedule);
                Schedules.Remove(SelectedSchedule);
                await _processService.UpdateProcessAsync(_process);
            }
        }

        private async Task CopyScheduleAsync()
        {
            if (SelectedSchedule == null) return;

            var copy = new Schedule
            {
                Name = $"{SelectedSchedule.Name} - Copy",
                Description = SelectedSchedule.Description,
                Author = SelectedSchedule.Author,
                ScheduleSafety = SelectedSchedule.ScheduleSafety,
                TargetSafety = SelectedSchedule.TargetSafety
            };

            foreach (var step in SelectedSchedule.Steps)
            {
                copy.Steps.Add(step);
            }

            _process.Schedules.Add(copy);
            Schedules.Add(copy);
            await _processService.UpdateProcessAsync(_process);
        }

        private async Task ImportScheduleAsync()
        {
            var filePath = await _fileService.SelectFileAsync("JSON Files (*.json)|*.json");
            if (string.IsNullOrEmpty(filePath)) return;

            var schedule = await _fileService.ImportScheduleAsync(filePath);
            if (schedule != null)
            {
                _process.Schedules.Add(schedule);
                Schedules.Add(schedule);
                await _processService.UpdateProcessAsync(_process);
                await _dialogService.ShowInfoAsync("스케줄을 성공적으로 가져왔습니다.");
            }
        }

        private async Task ExportScheduleAsync()
        {
            if (SelectedSchedule == null) return;

            var filePath = await _fileService.ExportScheduleAsync(SelectedSchedule);
            await _dialogService.ShowInfoAsync($"스케줄을 내보냈습니다: {filePath}");
        }

        private void SelectSchedule()
        {
            if (SelectedSchedule != null)
            {
                _mainViewModel.ShowScheduleEdit(SelectedSchedule);
            }
        }
    }
}
