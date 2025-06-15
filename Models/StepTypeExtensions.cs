using System;
using System.Collections.Generic;

namespace ScheduleEditor.Models
{
    public static class StepTypeExtensions
    {
        public static IEnumerable<StepType> GetAllStepTypes()
        {
            return Enum.GetValues<StepType>();
        }

        public static IEnumerable<StepMode> GetAllStepModes()
        {
            return Enum.GetValues<StepMode>();
        }

        public static IEnumerable<EndConditionType> GetAllEndConditionTypes()
        {
            return Enum.GetValues<EndConditionType>();
        }

        public static IEnumerable<ChamberMode> GetAllChamberModes()
        {
            return Enum.GetValues<ChamberMode>();
        }
    }
}