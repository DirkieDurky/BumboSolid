using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class AvailabilityRule
{
    public int Id { get; set; }

    public int Employee { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public byte Available { get; set; }

    public virtual AvailabilityDay AvailabilityDay { get; set; } = null!;
}
