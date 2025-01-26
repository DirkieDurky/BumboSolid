using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BumboSolid.Validations
{
	public class PasswordStrengthValidator : ValidationAttribute
	{
		private const string passwordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{}\\|`~;:'\",.<>])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{}\\|`~;:'\",.<>]{8,}$";

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is string password)
			{
				if (Regex.IsMatch(password, passwordRegex) == false)
				{
					return new ValidationResult("Wachtwoord is niet niet sterk genoeg. Zorg dat uw wachtwoord voldoet aan de volgende regels:\n" +
					"Minimaal 8 karakters.\n" +
					"Minimaal 1 cijfer.\n" +
					"Minimaal 1 kleine letter.\n" +
					"Minimaal 1 hoofdletter.\n" +
					"Minimaal 1 speciaal karakter. (Één van de volgende: !@#$%^&*()_+-=[]{}|`~)");
				}
			}

			return ValidationResult.Success;
		}
	}
}
