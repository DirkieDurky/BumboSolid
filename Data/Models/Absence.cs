namespace BumboSolid.Data.Models
{
    public class Absence
    {
        public int Id { get; set; }

        public int WeekId { get; set; }

        public byte Weekday { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? AbsentDescription { get; set; }

        public int? EmployeeId { get; set; }

        public User? Employee { get; set; } = null!;

        public virtual Week? Week { get; set; } = null!;
    }
}
