﻿using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class AvailabilityRuleViewModel
    {
		public String Day { get; set; }

		[DataType(DataType.Time)]
		public TimeOnly StartTime { get; set; }

		[DataType(DataType.Time)]
		public TimeOnly EndTime { get; set; }

		public bool Available { get; set; }

		public bool School { get; set; }
	}
}
