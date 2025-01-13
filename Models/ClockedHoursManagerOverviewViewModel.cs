using System.Collections.Generic;
using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
    public class ClockedHoursManagerOverviewViewModel
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public required List<ClockedHours> ClockedHours { get; set; }
        public required Dictionary<byte, string> WeekdayDictionary { get; set; }
        public int Year { get; set; }
        public int WeekNumber { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
    }
}
