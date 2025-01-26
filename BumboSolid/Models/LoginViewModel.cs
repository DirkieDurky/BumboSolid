using System.ComponentModel.DataAnnotations;
namespace BumboSolid.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vul een Email in.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vul een wachtwoord in.")]
    [DataType(DataType.Password, ErrorMessage = "Invalide waarde voor een wachtwoord")]
    [Display(Name = "Wachtwoord")]
    public string Password { get; set; }
}