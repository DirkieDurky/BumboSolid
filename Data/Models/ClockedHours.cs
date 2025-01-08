using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public class ClockedHours
{
	public int Id { get; set; }

	public int WeekId { get; set; }

	public byte Weekday { get; set; }

	public string Department { get; set; } = null!;

	public TimeOnly StartTime { get; set; }

	public TimeOnly? EndTime { get; set; }

	public int? EmployeeId { get; set; }

	public virtual User? Employee { get; set; } = null!;

	public string? ExternalEmployeeName { get; set; }

	public byte IsBreak { get; set; }

	public virtual Department? DepartmentNavigation { get; set; } = null!;

	public virtual Week? Week { get; set; } = null!;
}
