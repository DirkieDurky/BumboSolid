namespace BumboSolid.Models
{
    public class ShiftViewModel
    {
        public int Id { get; set; }

        public string Weekday { get; set; }

        public string Department { get; set; } = null!;


        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

    }
}
