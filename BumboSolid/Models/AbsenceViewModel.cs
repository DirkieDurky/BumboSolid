using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class AbsenceViewModel
    {
        public int ShiftId { get; set; }

        public string? Weekday { get; set; }

        public string? Department { get; set; }

        [Required(ErrorMessage = "Dit veld is vereist")]
        [DataType(DataType.Time, ErrorMessage = "Dit is geen valide tijd")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Dit veld is vereist")]
        [DataType(DataType.Time, ErrorMessage = "Dit is geen valide tijd")]
        public TimeOnly EndTime { get; set; }

        public string? Description { get; set; }
    }
}
