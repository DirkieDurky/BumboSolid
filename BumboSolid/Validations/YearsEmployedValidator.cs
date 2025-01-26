using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Validations
{
	public class YearsEmployedValidator : ValidationAttribute
	{
		private const int MinEmployedYears = 0;
		private const int MaxEmployedYears = 128;

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is DateOnly employedSince)
			{
				var yearsEmployed = DateTime.Today.Year - employedSince.Year;
				if (employedSince > DateOnly.FromDateTime(DateTime.Today.AddYears(-yearsEmployed)))
					yearsEmployed--;

				if (yearsEmployed < MinEmployedYears || yearsEmployed > MaxEmployedYears)
				{
					return new ValidationResult($"Dienstjaren moeten tussen {MinEmployedYears} en {MaxEmployedYears} jaar zijn. Huidige dienstjaren: {yearsEmployed}");
				}
			}

			return ValidationResult.Success;
		}
	}
}
