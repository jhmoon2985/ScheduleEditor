namespace ScheduleEditor.Models
{
    public class RecordCondition
    {
        public double TimeInterval { get; set; } = 1.0;
        public double VoltageChange { get; set; } = 0.001;
        public double CurrentChange { get; set; } = 0.001;
        public double TemperatureChange { get; set; } = 0.1;
    }
}