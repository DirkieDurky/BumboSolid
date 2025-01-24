using System.ComponentModel.DataAnnotations;
using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class HolidayManageViewModel
{
    [Required(ErrorMessage = "Dit veld is vereist")]
    public required Holiday Holiday { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly FirstDay { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [DataType(DataType.Date, ErrorMessage = "Invalide waarde voor een datum")]
    public DateOnly LastDay { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public int HighestImpact { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public int LowestImpact { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public List<String> xValues { get; set; } = new List<String>();

    [Required(ErrorMessage = "Dit veld is vereist")]
    public List<int> yValues { get; set; } = new List<int>();
}
