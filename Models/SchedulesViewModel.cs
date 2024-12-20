using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class SchedulesViewModel
	{
		public List<Week> Weeks = [];
		public int WeekId;
		public int? PreviousWeekId;
		public int? NextWeekId;
    }
}
