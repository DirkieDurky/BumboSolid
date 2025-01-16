using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class ScheduleViewDetailsViewModel
{
	public required List<Shift> Shifts { get; set; }

	public required string Day { get; set; }

	public TimeOnly StartTime { get; set; }

	public TimeOnly EndTime { get; set; }
}
