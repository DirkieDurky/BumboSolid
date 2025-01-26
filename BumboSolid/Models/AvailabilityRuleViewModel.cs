using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class AvailabilityRuleViewModel
	{
		public string Weekday { get; set; }

		public string Employee { get; set; }

		public TimeOnly StartTime { get; set; }

		public TimeOnly EndTime { get; set; }

		public byte Available { get; set; }

		public byte School { get; set; }
	}
}
