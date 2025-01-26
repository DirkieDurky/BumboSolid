using BumboSolid.Data.Models;
using BumboSolid.Validations;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class EmployeesEditViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [EmailAddress(ErrorMessage = "De ingevulde waarde voldoet niet aan de eisen voor een emailadres")]
    [StringLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
    public string Email { get; set; }

    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [StringLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
	[PasswordStrengthValidator]
	public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [StringLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
	[PasswordStrengthValidator]
	public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [StringLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
	[RegularExpression("^[a-zA-Z\\s':/]*$", ErrorMessage = "Alleen letters zijn toegestaan")]
	public string FirstName { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [StringLength(90, ErrorMessage = "De ingevulde waarde is te lang")]
	[RegularExpression("^[a-zA-Z\\s':/]*$", ErrorMessage = "Alleen letters zijn toegestaan")]
	public string LastName { get; set; }

    [StringLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? PlaceOfResidence { get; set; }

    [StringLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? StreetName { get; set; }

	[Range(0, 9999, ErrorMessage = "Het huisnummer moet een getal tussen 0 en 9999 zijn.")]
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
