using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class CLAManageViewModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Leeftijd moet een positief getal zijn")]
        public int? AgeStart { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Leeftijd moet een positief getal zijn")]
        public int? AgeEnd { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxWorkDurationPerDay { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxWorkDaysPerWeek { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxWorkDurationPerWeek { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxWorkDurationPerHolidayWeek { get; set; }

        public TimeOnly? EarliestWorkTime { get; set; }

        public TimeOnly? LatestWorkTime { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxShiftDuration { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxAvgWeeklyWorkDurationOverFourWeeks { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? BreakWorkDuration { get; set; }
       
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? BreakMinBreakDuration { get; set; }

        public bool MaxDayDurationHours { get; set; } = true;
        public bool MaxWeekDurationHours { get; set; } = true;
        public bool MaxHolidayDurationHours { get; set; } = true;
        public bool MaxAvgDurationHours { get; set; } = true;
        public bool MaxTotalShiftDurationHours { get; set; } = true;
        public bool MaxUninterruptedShiftDurationHours { get; set; } = true;
    }
}
