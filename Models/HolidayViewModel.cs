using BumboSolid.Data.Models;

namespace BumboSolid.Web.Models
{
	public class HolidayViewModel
	{
		public Holiday Holiday { get; set; }

		public DateOnly FirstDay { get; set; }

		public DateOnly LastDay { get; set; }
	}
}
