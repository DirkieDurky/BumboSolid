using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public class ClockedHours
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public int WeekId { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public byte Weekday { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public string Department { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? EmployeeId { get; set; }

    public virtual User? Employee { get; set; } = null!;

    public string? ExternalEmployeeName { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public byte IsBreak { get; set; }

    public virtual Department? DepartmentNavigation { get; set; } = null!;

    public virtual Week? Week { get; set; } = null!;
}
