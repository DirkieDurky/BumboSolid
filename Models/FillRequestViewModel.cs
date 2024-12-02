using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class FillRequestViewModel
    {
		public DateOnly Date { get; set; }

		public String Day { get; set; }

		public string Department { get; set; }

		public TimeOnly StartTime { get; set; }

		public TimeOnly EndTime { get; set; }

		public string? Status { get; set; }

		public string? Name { get; set; }

	}
}
