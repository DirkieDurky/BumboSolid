using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
    public class EmployeeScheduleDetailsViewModel
    {
        public List<Week> Weeks = [];
        public required string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public int WeekId { get; set; }
        public int? PreviousWeekId { get; set; }
        public int? NextWeekId { get; set; }
        public int CurrentWeekNumber { get; set; }
        public bool IsCurrentWeek { get; set; }
    }
}
