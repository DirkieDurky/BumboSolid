using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class ClockedHoursManagerViewModel
{
    public required ClockedHours ClockedHours { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }
}
