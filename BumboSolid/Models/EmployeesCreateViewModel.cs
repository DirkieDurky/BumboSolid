using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;
using BumboSolid.Validations;

namespace BumboSolid.Models;

public class EmployeesCreateViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Dit veld is vereist")]
    [EmailAddress(ErrorMessage = "De ingevulde waarde voldoet niet aan de eisen voor een emailadres")]
    [MaxLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [MaxLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
    [PasswordStrengthValidator]
    public string Password { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [Compare("Password", ErrorMessage = "Komt niet overeen met het wachtwoord.")]
    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [MaxLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
	[PasswordStrengthValidator]
	public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [MaxLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
	[RegularExpression("^[a-zA-Z\\s':/]*$", ErrorMessage = "Alleen letters zijn toegestaan")]
	public string FirstName { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [MaxLength(90, ErrorMessage = "De ingevulde waarde is te lang")]
	[RegularExpression("^[a-zA-Z\\s':/]*$", ErrorMessage = "Alleen letters zijn toegestaan")]
	public string LastName { get; set; }

    [MaxLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? PlaceOfResidence { get; set; }

    [MaxLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? StreetName { get; set; }

    [Range(0, 9999, ErrorMessage = "Het huisnummer moet een getal tussen 0 en 9999 zijn.")]
	[MaxLength(10, ErrorMessage = "De ingevulde waarde is te lang")]
	[RegularExpression("^[a-zA-Z0-9\\s':/]*$", ErrorMessage = "Alleen letters en cijfers zijn toegestaan")]
	public int? StreetNumber { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date)]
	[DateRangeValidator]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
	[DataType(DataType.Date)]
	[DateRangeValidator]
	public DateOnly EmployedSince { get; set; }

    public List<Department> Departments { get; set; } = new List<Department>();

    public List<string> SelectedDepartments { get; set; } = new List<string>();

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// Check if the EmployedSince date is not before BirthDate
		if (EmployedSince < BirthDate) yield return new ValidationResult("De datum 'In dienst sinds' kan niet eerder zijn dan de geboortedatum");

		// Check if Password is the same as ConfirmPassword
		if (Password != ConfirmPassword)
		{
			yield return new ValidationResult("De wachtwoorden komen niet overeen");
		}

		// Check if any Department has been selected
		if (!SelectedDepartments.Any())
		{
			yield return new ValidationResult("Kies minstens één afdeling.");
		}

		yield return ValidationResult.Success;
	}
}