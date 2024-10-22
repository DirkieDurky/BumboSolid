using BumboSolid.Data.Models;

namespace BumboSolid.Web.Models
{
	public class HolidayViewModel
	{
		public String Name { get; set; }

		public DateOnly FirstDay { get; set; }

		public DateOnly LastDay { get; set; }
	}
}
