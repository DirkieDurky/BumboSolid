using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class AbsenceViewModel
    {
        public int ShiftId { get; set; }

        public string? Weekday { get; set; }

        public string? Department { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        public string? Description { get; set; }
    }
}
