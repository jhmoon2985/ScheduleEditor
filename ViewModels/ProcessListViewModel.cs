using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;
using ScheduleEditor.ViewModels.Base;

namespace ScheduleEditor.ViewModels
{
    public class ProcessListViewModel : ViewModelBase
    {
        private readonly IProcessService _processService;
        private readonly IDialogService _dialogService;
        private readonly MainViewModel _mainViewModel;

        private Process? _selectedProcess;

        public ProcessListViewModel(
            IProcessService processService,
            IDialogService dialogService,
            MainViewModel mainViewModel)
        {
            _processService = processService;
            _dialogService = dialogService;
            _mainViewModel = mainViewModel;

            Processes = new ObservableCollection<Process>();

            AddProcessCommand = new RelayCommand(async () => await AddProcessAsync());
            EditProcessCommand = new RelayCommand(async () => await EditProcessAsync(), () => SelectedProcess != null);
            DeleteProcessCommand = new RelayCommand(async () => await DeleteProcessAsync(), () => SelectedProcess != null);
            CopyProcessCommand = new RelayCommand(async () => await CopyProcessAsync(), () => SelectedProcess != null);
            SelectProcessCommand = new RelayCommand(SelectProcess, () => SelectedProcess != null);

            LoadProcessesAsync();
        }

        public ObservableCollection<Process> Processes { get; }

        public Process? SelectedProcess
        {
            get => _selectedProcess;
            set => SetProperty(ref _selectedProcess, value);
        }

        public ICommand AddProcessCommand { get; }
        public ICommand EditProcessCommand { get; }
        public ICommand DeleteProcessCommand { get; }
        public ICommand CopyProcessCommand { get; }
        public ICommand SelectProcessCommand { get; }

        private async Task LoadProcessesAsync()
        {
            var processes = await _processService.GetAllProcessesAsync();
            Processes.Clear();
            foreach (var process in processes)
            {
                Processes.Add(process);
            }
        }

        private async Task AddProcessAsync()
        {
            var name = await _dialogService.ShowInputDialogAsync("공정 등록", "공정명을 입력하세요:");
            if (string.IsNullOrEmpty(name)) return;

            var description = await _dialogService.ShowInputDialogAsync("공정 등록", "설명을 입력하세요:");
            var author = await _dialogService.ShowInputDialogAsync("공정 등록", "작성자를 입력하세요:");

            var process = await _processService.CreateProcessAsync(name, description ?? "", author ?? "");
            Processes.Add(process);
        }

        private async Task EditProcessAsync()
        {
            if (SelectedProcess == null) return;

            var name = await _dialogService.ShowInputDialogAsync("공정 수정", "공정명:", SelectedProcess.Name);
            if (string.IsNullOrEmpty(name)) return;

            var description = await _dialogService.ShowInputDialogAsync("공정 수정", "설명:", SelectedProcess.Description);
            var author = await _dialogService.ShowInputDialogAsync("공정 수정", "작성자:", SelectedProcess.Author);

            SelectedProcess.Name = name;
            SelectedProcess.Description = description ?? "";
            SelectedProcess.Author = author ?? "";

            await _processService.UpdateProcessAsync(SelectedProcess);
        }

        private async Task DeleteProcessAsync()
        {
            if (SelectedProcess == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "공정 삭제",
                $"'{SelectedProcess.Name}' 공정을 삭제하시겠습니까?");

            if (confirmed)
            {
                await _processService.DeleteProcessAsync(SelectedProcess.Id);
                Processes.Remove(SelectedProcess);
            }
        }

        private async Task CopyProcessAsync()
        {
            if (SelectedProcess == null) return;

            var copy = await _processService.CopyProcessAsync(SelectedProcess);
            Processes.Add(copy);
        }

        private void SelectProcess()
        {
            if (SelectedProcess != null)
            {
                _mainViewModel.ShowScheduleList(SelectedProcess);
            }
        }
    }
}