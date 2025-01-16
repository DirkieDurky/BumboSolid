using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class CLAManageViewModel
    {
        public int? Id { get; set; }
        
        [Range(0, 128, ErrorMessage = "Leeftijd moet een getal tussen 0 en 128 zijn")]
        public int? AgeStart { get; set; }

        [Range(0, 128, ErrorMessage = "Leeftijd moet een getal tussen 0 en 128 zijn")]
        public int? AgeEnd { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? MaxWorkDurationPerDay { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        public int? MaxWorkDaysPerWeek { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? MaxWorkDurationPerWeek { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? MaxWorkDurationPerHolidayWeek { get; set; }

        public TimeOnly? EarliestWorkTime { get; set; }

        public TimeOnly? LatestWorkTime { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? MaxShiftDuration { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? MaxAvgWeeklyWorkDurationOverFourWeeks { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? BreakWorkDuration { get; set; }
       
        [Range(0, int.MaxValue, ErrorMessage = "Werkduur mag niet negatief zijn.")]
        [RegularExpression(@"^\d+([\.\,](?:\d|(25)|(75)))?$", ErrorMessage = "Graag maximaal 1 kommagetal, of kwarten")]
        public decimal? BreakMinBreakDuration { get; set; }

        public bool MaxDayDurationHours { get; set; } = true;
        public bool MaxWeekDurationHours { get; set; } = true;
        public bool MaxHolidayDurationHours { get; set; } = true;
        public bool MaxAvgDurationHours { get; set; } = true;
        public bool MaxTotalShiftDurationHours { get; set; } = true;
        public bool MaxUninterruptedShiftDurationHours { get; set; } = true;
        public bool MinBreakTimeHours { get; set; } = false;
    }
}
