using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class EmployeesCreateViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
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

        public int? StreetNumber { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        public DateOnly EmployedSince { get; set; }

        public List<Department> Departments { get; set; } = new List<Department>();

        [Required(ErrorMessage = "Kies minstens één afdeling.")]
        public List<string> SelectedDepartments { get; set; } = new List<string>();
    }
}
