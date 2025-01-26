using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Validations
{
	public class DateRangeValidator : ValidationAttribute
	{
		private const int MinYears = 0;
		private const int MaxYears = 128;

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is DateOnly date)
			{
				var age = DateTime.Today.Year - date.Year;
				if (date > DateOnly.FromDateTime(DateTime.Today.AddYears(-age)))
					age--;

				if (age < MinYears || age > MaxYears)
				{
					return new ValidationResult($"Jaren moeten tussen {MinYears} en {MaxYears} jaar zijn. Huidige jaren: {age}");
				}
			}

			return ValidationResult.Success;
		}
	}
}
