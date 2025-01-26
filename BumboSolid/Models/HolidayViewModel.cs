using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class HolidayViewModel
{
    [Required(ErrorMessage = "Dit veld is vereist")]
    [StringLength(25, ErrorMessage = "De ingevulde waarde is te lang")]
    [RegularExpression("^[a-zA-Z0-9\\s]*$", ErrorMessage = "Alleen alfanumerieke tekens en spaties zijn toegestaan.")]
    public String Name { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly FirstDay { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly LastDay { get; set; }
}
