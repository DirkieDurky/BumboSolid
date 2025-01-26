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
        foreach (var item in VisitorEstimates) if (item > 10000 || item < 0) yield return new ValidationResult("De waarde geschatte bezoekersdrukte mag niet hoger zijn dan 10000 en lager dan 0");
		foreach (var item in Holidays) if (item > 100000 || item < -10000) yield return new ValidationResult("De waarde feestdagen mag niet hoger of lager zijn dan 100000");
		foreach (var item in WeatherIds) if (item > 10000 || item < -10000) yield return new ValidationResult("De waarde weer mag niet hoger of lager zijn dan 10000");
		foreach (var item in Others) if (item > 10000 || item < -10000) yield return new ValidationResult("De waarde over mag niet hoger of lager zijn dan 10000");

		yield return ValidationResult.Success;
	}
}