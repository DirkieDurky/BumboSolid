using BumboSolid.Data.Models;
using System;
using System.Collections.Generic;

namespace BumboSolid.Models;

public class ScheduleViewDetailsViewModel
{
	public required List<Shift> Shifts { get; set; }

	public required string Day { get; set; }

	public TimeOnly StartTime { get; set; }

	public TimeOnly EndTime { get; set; }
}
