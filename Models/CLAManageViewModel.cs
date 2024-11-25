using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
    public class CLAManageViewModel
    {
        public int? AgeStart { get; set; }

        public int? AgeEnd { get; set; }

        public int? MaxWorkDurationPerDay { get; set; }

        public int? MaxWorkDaysPerWeek { get; set; }

        public int? MaxWorkDurationPerWeek { get; set; }

        public int? MaxWorkDurationPerHolidayWeek { get; set; }

        public TimeOnly? EarliestWorkTime { get; set; }

        public TimeOnly? LatestWorkTime { get; set; }

        public int? MaxAvgWeeklyWorkDurationOverFourWeeks { get; set; }

        public int? MaxShiftDuration { get; set; }
        public int? BreakWorkDuration { get; set; }
        public int? BreakMinBreakDuration { get; set; }
    }
}
