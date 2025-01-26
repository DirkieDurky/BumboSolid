using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class ShiftCreateViewModel : IValidatableObject
{
    public required Shift Shift { get; set; }

    public List<User>? Employees { get; set; } = null!;

    public List<CLAEntry>? CLAEntries { get; set; } = null!;

    public List<Shift>? Shifts { get; set; } = null!;

    public required Week? Week { get; set; }

    public List<Department>? Departments { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// Check if EndTime is not Before StartTime
		if (Shift.EndTime <= Shift.StartTime) yield return new ValidationResult("De eindtijd moet later zijn dan de starttijd");
		
		yield return ValidationResult.Success;
	}
}