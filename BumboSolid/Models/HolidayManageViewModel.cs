using System.ComponentModel.DataAnnotations;
using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class HolidayManageViewModel : IValidatableObject
{
    public required Holiday Holiday { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly FirstDay { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly LastDay { get; set; }

    public int HighestImpact { get; set; }

    public int LowestImpact { get; set; }

    public List<String> xValues { get; set; } = new List<String>();

    public List<int> yValues { get; set; } = new List<int>();

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
        // Check if FirstDay is not before LastDay
		if (LastDay < FirstDay) yield return new ValidationResult("Begintijd moet hetzelfde of later zijn dan eindtijd");

		// Making sure that FirstDay and LastDay are still in the same year
		if (FirstDay.Year != Holiday.HolidayDays[0].Date.Year || LastDay.Year != Holiday.HolidayDays[0].Date.Year)
		{
			yield return new ValidationResult($"Het gegeven jaar moet hetzelfde zijn als het jaar waarin het feest is gemaakt: {Holiday.HolidayDays[0].Date.Year}");
		}

		// Make sure not both dates are changed at the same time
		int firstDayDifference = FirstDay.DayNumber - Holiday.HolidayDays[0].Date.DayNumber;
		int LastDayDifference = LastDay.DayNumber - Holiday.HolidayDays[Holiday.HolidayDays.Count() - 1].Date.DayNumber;
		if (firstDayDifference != 0 && LastDayDifference != 0)
		{
			yield return new ValidationResult("Je kan niet beide dagen tergelijkertijd aanpassen");
		}

		yield return ValidationResult.Success;
	}
}