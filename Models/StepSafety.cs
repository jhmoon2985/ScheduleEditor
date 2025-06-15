namespace ScheduleEditor.Models
{
    public class StepSafety
    {
        public bool IsEnabled { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int ReferenceStep { get; set; }
        public StepSafetyType Type { get; set; }
    }

    public enum StepSafetyType
    {
        Voltage,
        Current,
        Power,
        Temperature,
        SOH,
        Capacity
    }
}