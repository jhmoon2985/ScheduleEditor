using ScheduleEditor.ViewModels.Base;

namespace ScheduleEditor.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private double _maxVoltage = 5.0;
        private double _maxCurrent = 110.0;
        private double _maxPower = 550.0;
        private double _maxTemperature = 60.0;
        private double _minTemperature = -20.0;

        public double MaxVoltage
        {
            get => _maxVoltage;
            set => SetProperty(ref _maxVoltage, value);
        }

        public double MaxCurrent
        {
            get => _maxCurrent;
            set => SetProperty(ref _maxCurrent, value);
        }

        public double MaxPower
        {
            get => _maxPower;
            set => SetProperty(ref _maxPower, value);
        }

        public double MaxTemperature
        {
            get => _maxTemperature;
            set => SetProperty(ref _maxTemperature, value);
        }

        public double MinTemperature
        {
            get => _minTemperature;
            set => SetProperty(ref _minTemperature, value);
        }
    }
}