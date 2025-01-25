using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class AvailabilityRule
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public int Employee { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public TimeOnly EndTime { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public byte Available { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public byte School { get; set; }

    public virtual User? EmployeeNavigation { get; set; } = null!;
}
