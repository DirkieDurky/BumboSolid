﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class AvailabilityRule
{
    public int Employee { get; set; }

	[DataType(DataType.Date)]
	public DateOnly Date { get; set; }

	[DataType(DataType.Time)]
	public TimeOnly StartTime { get; set; }

	[DataType(DataType.Time)]
	public TimeOnly EndTime { get; set; }

    public byte Available { get; set; }

    public byte School { get; set; }

    public virtual Employee EmployeeNavigation { get; set; } = null!;
}
