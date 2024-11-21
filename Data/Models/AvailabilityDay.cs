using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class AvailabilityDay
{
    public int Employee { get; set; }

    public DateOnly Date { get; set; }

    public int? LessonHours { get; set; }

    public virtual List<AvailabilityRule> AvailabilityRules { get; set; } = new List<AvailabilityRule>();

    public virtual Employee EmployeeNavigation { get; set; } = null!;
}
