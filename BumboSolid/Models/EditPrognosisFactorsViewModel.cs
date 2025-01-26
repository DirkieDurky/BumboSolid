using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class EditPrognosisFactorsViewModel : IValidatableObject
{
    public required Week Prognosis { get; set; }

    public required List<Weather> WeatherValues { get; set; }

    // These properties hold the user input
    [Required(ErrorMessage = "Dit veld is vereist")]
    public int[] VisitorEstimates { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
	public int[] Holidays { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public int[] WeatherIds { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public int[] Others { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public string[] Descriptions { get; set; } = null!;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// Check if the proporties don't go out of range
        foreach (var item in VisitorEstimates) if (item > 10000) yield return new ValidationResult("De waardes mogen niet hoger zijn dan 10000");

		yield return ValidationResult.Success;
	}
}