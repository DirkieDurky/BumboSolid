namespace BumboSolid.Data.Models;

public partial class CLAEntry
{
    public int Id { get; set; }

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

	public int? HolidaySurcharge { get; set; }

    public virtual List<CLABreakEntry> CLABreakEntries { get; set; } = new List<CLABreakEntry>();

    public virtual List<CLASurchargeEntry> CLASurchargeEntries { get; set; } = [];
}
