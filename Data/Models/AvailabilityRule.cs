using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class AvailabilityRule
{
	public int Id { get; set; }

	public int Employee { get; set; }

	public DateOnly Date { get; set; }

	public TimeOnly StartTime { get; set; }

	public TimeOnly EndTime { get; set; }

	public byte Available { get; set; }

	public byte School { get; set; }

	public virtual Employee EmployeeNavigation { get; set; } = null!;
}
