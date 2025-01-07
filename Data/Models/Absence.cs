namespace BumboSolid.Data.Models
{
    public class Absence
    {
        public int Id { get; set; }

        public int ShiftId { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? AbsentDescription { get; set; }

        public int? EmployeeId { get; set; }

		public virtual Shift Shift { get; set; } = null!;

		public virtual User? Employee { get; set; }
	}
}
