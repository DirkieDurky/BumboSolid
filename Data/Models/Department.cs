using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Department
{
    public string Name { get; set; } = null!;

    public virtual List<Norm> Norms { get; set; } = new List<Norm>();

    public virtual List<PrognosisDepartment> PrognosisDepartments { get; set; } = new List<PrognosisDepartment>();

    public virtual List<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual List<ClockedHours> ClockedHours { get; set; } = new List<ClockedHours>();


    public virtual List<User> Employees { get; set; } = new List<User>();
}
