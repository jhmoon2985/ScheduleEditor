namespace ScheduleEditor.Models
{
    public class EndCondition
    {
        public EndConditionType Type { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int MoveStep { get; set; }
        public int ReferenceStep { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public enum EndConditionType
    {
        VoltageHigh,
        VoltageLow,
        Current,
        Time,
        Capacity,
        WattHour,
        Power,
        CVTime,
        Temperature,
        SOC,
        DOD,
        CycleCapacity,
        CycleEnergy,
        GroupCapacity,
        GroupEnergy,
        CycleCount,
        GroupCycleCount,
        DeRating
    }
}