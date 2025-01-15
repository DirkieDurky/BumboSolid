using BumboSolid.Data.Models;

namespace BumboSolid.Models;

public class ShiftCreateViewModel
{
    public required Shift Shift { get; set; }

    public required List<User>? Employees { get; set; }

    public required Week? Week { get; set; }

    public List<Department>? Departments { get; set; }
}
