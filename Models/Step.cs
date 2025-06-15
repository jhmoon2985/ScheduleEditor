using System.Collections.ObjectModel;
using System.Windows.Automation;

namespace ScheduleEditor.Models
{
    public class Step
    {
        public int StepNumber { get; set; }
        public StepType Type { get; set; } = StepType.Rest;
        public StepMode Mode { get; set; } = StepMode.CC;
        public double ChargeVoltage { get; set; }
        public double DischargeVoltage { get; set; }
        public double Current { get; set; }
        public double Power { get; set; }
        public Pattern Pattern { get; set; } = new();
        public ObservableCollection<EndCondition> EndConditions { get; set; } = new();
        public ObservableCollection<StepSafety> StepSafeties { get; set; } = new();
        public RecordCondition RecordCondition { get; set; } = new();
        public ChamberSetting ChamberSetting { get; set; } = new();
        public int LoopCount { get; set; } = 1;
        public int GroupId { get; set; }
    }
}