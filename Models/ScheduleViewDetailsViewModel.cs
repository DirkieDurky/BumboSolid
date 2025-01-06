using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class ScheduleViewDetailsViewModel
{
	public required List<Shift> Shifts { get; set; }

	public byte Year { get; set; }
	public byte Week { get; set; }
}
