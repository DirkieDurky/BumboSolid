using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class ShiftCreateViewModel
{
    public required Shift Shift { get; set; }

    public List<User>? Employees { get; set; } = null!;

    public List<CLAEntry>? CLAEntries { get; set; } = null!;

    public List<Shift>? Shifts { get; set; } = null!;

    public required Week? Week { get; set; }

    public List<Department>? Departments { get; set; }
}
