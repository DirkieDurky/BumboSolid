using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class CLASurchargeEntry
{
	public int Id { get; set; }
	[Range(1, 1000, ErrorMessage = $"Toeslag moet groter dan 0 zijn en lager of gelijk aan 1000")]
	public int Surcharge { get; set; }
	public byte? Weekday { get; set; }
	public TimeOnly? StartTime { get; set; }
	public TimeOnly? EndTime { get; set; }

	// JANO SURCHARGE ENTRY MOET MINIMAAL 1 VAN VOLGENDE HEBBEN WEEK OF (STARTTIME EN ENDTIME)

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (StartTime.HasValue && EndTime.HasValue && StartTime > EndTime)
		{
			yield return new ValidationResult("StartTime must be earlier than EndTime.", [nameof(StartTime), nameof(EndTime)]);
		}

		yield return ValidationResult.Success;
	}
}
