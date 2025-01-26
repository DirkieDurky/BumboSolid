using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class PrognosesViewModel
{
    public required Week Prognose { get; set; }
    public required List<Week> Weeks = [];
    public int WeekId { get; set; }
    public int? PreviousWeekId { get; set; }
    public int? NextWeekId { get; set; }
    public int CurrentWeekNumber { get; set; }
    public bool IsCurrentWeek { get; set; }
}
