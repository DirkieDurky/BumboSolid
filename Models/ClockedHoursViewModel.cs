using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class ClockedHoursViewModel
	{
		public DateOnly StartDate { get; set; }

		public DateOnly EndDate { get; set; }

		public virtual List<ClockedHours> ClockedHours { get; set; } = new List<ClockedHours>();

		public Dictionary<byte, string> WeekdayDictionary { get; set; }

		public List<Department> Departments { get; set; } = new List<Department>();

		public List<string> SelectedDepartments { get; set; } = new List<string>();

		public Department? LastDepartment { get; set; }

		public int WeekId { get; set; }
	}
}
