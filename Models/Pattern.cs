using System.Collections.ObjectModel;

namespace ScheduleEditor.Models
{
    public class Pattern
    {
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public double VoltageHigh { get; set; }
        public double VoltageLow { get; set; }
        public PatternMode Mode { get; set; } = PatternMode.CC;
        public PatternTimeMode TimeMode { get; set; } = PatternTimeMode.Cumulative;
        public int RepeatCount { get; set; } = 1;
        public int LineCount { get; set; }
        public double TimeInterval { get; set; } = 1.0;
        public ObservableCollection<PatternData> Data { get; set; } = new();
    }

    public class PatternData
    {
        public double Time { get; set; }
        public double Value { get; set; }
    }

    public enum PatternMode
    {
        CC,
        CV,
        CP
    }

    public enum PatternTimeMode
    {
        Cumulative,
        Operating
    }
}