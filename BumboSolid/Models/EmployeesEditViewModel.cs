using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class EmployeesEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [EmailAddress(ErrorMessage = "De ingevulde waarde voldoet niet aan de eisen voor een emailadres")]
    [StringLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
    public string Email { get; set; }

    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [StringLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [StringLength(256, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [StringLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [StringLength(90, ErrorMessage = "De ingevulde waarde is te lang")]
    public string LastName { get; set; }

    [StringLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? PlaceOfResidence { get; set; }

    [StringLength(45, ErrorMessage = "De ingevulde waarde is te lang")]
    public string? StreetName { get; set; }

    [Range(0, 9999, ErrorMessage = "Het huisnummer moet een getal tussen 0 en 9999 zijn.")]
    public int? StreetNumber { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public DateOnly EmployedSince { get; set; }

    public List<Department> Departments { get; set; } = new List<Department>();

    [Required(ErrorMessage = "Kies minstens één afdeling.")]
    public List<string> SelectedDepartments { get; set; } = new List<string>();
}
