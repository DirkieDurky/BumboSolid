using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class EmployeesCreateViewModel
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(256)]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Komt niet overeen met het wachtwoord.")]
    [DataType(DataType.Password)]
    [StringLength(256)]
    public string ConfirmPassword { get; set; }

    [Required]
    [StringLength(45)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(90)]
    public string LastName { get; set; }

    [StringLength(45)]
    public string? PlaceOfResidence { get; set; }

    [StringLength(45)]
    public string? StreetName { get; set; }

    [Range(0, 9999, ErrorMessage = "Het huisnummer moet een getal tussen 0 en 9999 zijn.")]
    public int? StreetNumber { get; set; }

    [Required]
    public DateOnly BirthDate { get; set; }

    [Required]
    public DateOnly EmployedSince { get; set; }

    public List<Department> Departments { get; set; } = new List<Department>();

    [Required(ErrorMessage = "Kies minstens één afdeling.")]
    public List<string> SelectedDepartments { get; set; } = new List<string>();
}
