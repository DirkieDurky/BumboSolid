using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class HolidayViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Dit veld is vereist")]
    [StringLength(25, ErrorMessage = "De ingevulde waarde is te lang")]
    [RegularExpression("^[a-zA-Z0-9\\s]*$", ErrorMessage = "Alleen alfanumerieke tekens en spaties zijn toegestaan.")]
    public String Name { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly FirstDay { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly LastDay { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// Check if FirstDay is not before LastDay
		if (LastDay < FirstDay) yield return new ValidationResult("Begintijd moet hetzelfde of later zijn dan eindtijd");
		
		yield return ValidationResult.Success;
	}
}