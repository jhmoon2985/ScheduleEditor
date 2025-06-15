namespace ScheduleEditor.Models
{
    public class ChamberSetting
    {
        public bool StepWait { get; set; }
        public double Temperature { get; set; } = 25.0;
        public ChamberMode Mode { get; set; } = ChamberMode.Off;
    }

    public enum ChamberMode
    {
        Off,
        Heat,
        Cool,
        Auto
    }
}