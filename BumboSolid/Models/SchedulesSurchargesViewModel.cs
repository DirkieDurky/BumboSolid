using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class SchedulesSurchargesViewModel
{
    public List<Week> Weeks = [];

    public int WeekId { get; set; }

    public int? PreviousWeekId { get; set; }

    public int? NextWeekId { get; set; }

    public bool IsCurrentWeek { get; set; }

    public bool HasSchedule { get; set; }

    public List<Department>? Departments { get; set; }

    public List<CLASurchargeEntry>? Surcharges { get; set; }

    public List<ClockedHours>? ClockedHours { get; set; }
}