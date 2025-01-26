namespace BumboSolid.Data.Models;

public partial class Department
{
    public string Name { get; set; } = null!;

    public virtual List<Norm> Norms { get; set; } = [];

    public virtual List<PrognosisDepartment> PrognosisDepartments { get; set; } = [];

    public virtual List<Shift> Shifts { get; set; } = [];

    public virtual List<ClockedHours> ClockedHours { get; set; } = [];

    public virtual List<User> Employees { get; set; } = [];
}
