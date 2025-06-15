using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;
using ScheduleEditor.ViewModels.Base;

namespace ScheduleEditor.ViewModels
{
    public class ScheduleEditViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly MainViewModel _mainViewModel;
        private readonly Schedule _schedule;

        private Step? _selectedStep;

        public ScheduleEditViewModel(
            IDialogService dialogService,
            MainViewModel mainViewModel,
            Schedule schedule)
        {
            _dialogService = dialogService;
            _mainViewModel = mainViewModel;
            _schedule = schedule;

            Steps = new ObservableCollection<Step>(_schedule.Steps);

            AddStepCommand = new RelayCommand(AddStep);
            DeleteStepCommand = new RelayCommand(DeleteStep, () => SelectedStep != null);
            CopyStepCommand = new RelayCommand(CopyStep, () => SelectedStep != null);
            PasteStepCommand = new RelayCommand(PasteStep, () => _copiedStep != null);
            MoveUpCommand = new RelayCommand(MoveUp, () => SelectedStep != null && Steps.IndexOf(SelectedStep) > 0);
            MoveDownCommand = new RelayCommand(MoveDown, () => SelectedStep != null && Steps.IndexOf(SelectedStep) < Steps.Count - 1);

            // 기본 스텝이 없으면 추가
            if (!Steps.Any())
            {
                AddDefaultSteps();
            }
        }

        public ObservableCollection<Step> Steps { get; }
        public Schedule Schedule => _schedule;
        public string ScheduleName => _schedule.Name;

        public Step? SelectedStep
        {
            get => _selectedStep;
            set => SetProperty(ref _selectedStep, value);
        }

        public ICommand AddStepCommand { get; }
        public ICommand DeleteStepCommand { get; }
        public ICommand CopyStepCommand { get; }
        public ICommand PasteStepCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }

        private Step? _copiedStep;

        private void AddStep()
        {
            var step = new Step
            {
                StepNumber = Steps.Count + 1,
                Type = StepType.Rest,
                Mode = StepMode.CC
            };

            Steps.Add(step);
            _schedule.Steps.Add(step);
            UpdateStepNumbers();
        }

        private void DeleteStep()
        {
            if (SelectedStep == null) return;

            Steps.Remove(SelectedStep);
            _schedule.Steps.Remove(SelectedStep);
            UpdateStepNumbers();
        }

        private void CopyStep()
        {
            if (SelectedStep == null) return;
            _copiedStep = SelectedStep;
        }

        private void PasteStep()
        {
            if (_copiedStep == null) return;

            var copy = new Step
            {
                StepNumber = Steps.Count + 1,
                Type = _copiedStep.Type,
                Mode = _copiedStep.Mode,
                ChargeVoltage = _copiedStep.ChargeVoltage,
                DischargeVoltage = _copiedStep.DischargeVoltage,
                Current = _copiedStep.Current,
                Power = _copiedStep.Power,
                Pattern = _copiedStep.Pattern,
                RecordCondition = _copiedStep.RecordCondition,
                ChamberSetting = _copiedStep.ChamberSetting,
                LoopCount = _copiedStep.LoopCount,
                GroupId = _copiedStep.GroupId
            };

            foreach (var endCondition in _copiedStep.EndConditions)
            {
                copy.EndConditions.Add(endCondition);
            }

            foreach (var safety in _copiedStep.StepSafeties)
            {
                copy.StepSafeties.Add(safety);
            }

            Steps.Add(copy);
            _schedule.Steps.Add(copy);
            UpdateStepNumbers();
        }

        private void MoveUp()
        {
            if (SelectedStep == null) return;

            var index = Steps.IndexOf(SelectedStep);
            if (index > 0)
            {
                Steps.Move(index, index - 1);
                var scheduleIndex = _schedule.Steps.IndexOf(SelectedStep);
                _schedule.Steps.Move(scheduleIndex, scheduleIndex - 1);
                UpdateStepNumbers();
            }
        }

        private void MoveDown()
        {
            if (SelectedStep == null) return;

            var index = Steps.IndexOf(SelectedStep);
            if (index < Steps.Count - 1)
            {
                Steps.Move(index, index + 1);
                var scheduleIndex = _schedule.Steps.IndexOf(SelectedStep);
                _schedule.Steps.Move(scheduleIndex, scheduleIndex + 1);
                UpdateStepNumbers();
            }
        }

        private void UpdateStepNumbers()
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].StepNumber = i + 1;
            }
        }

        private void AddDefaultSteps()
        {
            // Cycle 시작
            var cycleStep = new Step
            {
                StepNumber = 1,
                Type = StepType.Cycle,
                Mode = StepMode.CC
            };
            Steps.Add(cycleStep);
            _schedule.Steps.Add(cycleStep);

            // Loop 종료
            var loopStep = new Step
            {
                StepNumber = 2,
                Type = StepType.Loop,
                Mode = StepMode.CC,
                LoopCount = 1
            };
            loopStep.EndConditions.Add(new EndCondition
            {
                Type = EndConditionType.CycleCount,
                Value = 1,
                Unit = "회"
            });
            Steps.Add(loopStep);
            _schedule.Steps.Add(loopStep);

            // Finish
            var finishStep = new Step
            {
                StepNumber = 3,
                Type = StepType.Finish,
                Mode = StepMode.CC
            };
            Steps.Add(finishStep);
            _schedule.Steps.Add(finishStep);
        }
    }
}