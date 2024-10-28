using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class HolidayManageViewModel
	{
		public Holiday Holiday { get; set; }

		public DateOnly FirstDay { get; set; }

		public DateOnly LastDay { get; set; }

		public int HighestImpact { get; set; }

		public int LowestImpact { get; set; }

		public List<String> xValues { get; set; } = new List<String>();

		public List<int> yValues { get; set; } = new List<int>();
	}
}
