using System.ComponentModel.DataAnnotations;
using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class HolidayManageViewModel
	{
		public required Holiday Holiday { get; set; }

        [DataType(DataType.Date)]
        public DateOnly FirstDay { get; set; }

        [DataType(DataType.Date)]
        public DateOnly LastDay { get; set; }

		public int HighestImpact { get; set; }

		public int LowestImpact { get; set; }

		public List<String> xValues { get; set; } = new List<String>();

		public List<int> yValues { get; set; } = new List<int>();
	}
}
