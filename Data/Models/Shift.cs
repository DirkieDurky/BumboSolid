using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Shift
{
    public int Id { get; set; }

    public int WeekId { get; set; }

    public byte Weekday { get; set; }

    public string Department { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int? Employee { get; set; }

    public string? ExternalEmployeeName { get; set; }

    public virtual Department DepartmentNavigation { get; set; } = null!;

    public virtual List<FillRequest> FillRequests { get; set; } = new List<FillRequest>();

    public virtual Week Week { get; set; } = null!;
}
