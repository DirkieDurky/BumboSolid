namespace BumboSolid.Data.Models
{
    public class Absence
    {
        public int Id { get; set; }

        public int WeekId { get; set; }

        public int Weekday { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? AbsentDescription { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Week? Week { get; set; } = null!;

        public virtual User? Employee { get; set; }
    }
}
