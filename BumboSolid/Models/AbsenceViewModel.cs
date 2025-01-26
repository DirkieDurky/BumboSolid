using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
	public class AbsenceViewModel : IValidatableObject
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

		[MaxLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
		public string? Description { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (StartTime > EndTime) yield return new ValidationResult("De laatste dag moet hetzelfde of later zijn dan de eerste dag");

			yield return ValidationResult.Success;
		}
	}
}