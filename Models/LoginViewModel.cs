using System.ComponentModel.DataAnnotations;
namespace BumboSolid.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vul een Email in.")]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vul een wachtwoord in.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}