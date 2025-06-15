using System;
using System.Collections.ObjectModel;

namespace ScheduleEditor.Models
{
    public class Schedule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObservableCollection<Step> Steps { get; set; } = new();
        public ScheduleSafety ScheduleSafety { get; set; } = new();
        public TargetSafety TargetSafety { get; set; } = new();
    }

    public class ScheduleSafety
    {
        public double MaxVoltage { get; set; } = 5.0;
        public double MaxCurrent { get; set; } = 110.0;
        public double MaxPower { get; set; } = 550.0;
        public double MaxTemperature { get; set; } = 60.0;
        public double MinTemperature { get; set; } = -20.0;
    }

    public class TargetSafety
    {
        public double TargetVoltage { get; set; }
        public double TargetCurrent { get; set; }
        public double TargetPower { get; set; }
        public double TargetTemperature { get; set; }
    }
}