using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class ClockedHoursManagerOverviewViewModel
{
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public required List<ClockedHours> ClockedHours { get; set; }

    public required Dictionary<byte, string> WeekdayDictionary { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public int WeekId { get; set; }
    public int? PreviousWeekId { get; set; }
    public int? NextWeekId { get; set; }
    public bool IsCurrentWeek { get; set; }
    public bool HasSchedule { get; set; }
}
