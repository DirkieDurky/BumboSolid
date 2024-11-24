using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models
{
    public class EmployeesCreateViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public string? PlaceOfResidence { get; set; }

        public string? StreetName { get; set; }

        public int? StreetNumber { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        public DateOnly EmployedSince { get; set; }
    }
}
