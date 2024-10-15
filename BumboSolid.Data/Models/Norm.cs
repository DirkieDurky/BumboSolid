using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Norm
{
    public int Id { get; set; }

    public string Activity { get; set; } = null!;

    public string Function { get; set; } = null!;

    public int Duration { get; set; }

    public byte AvgDailyPerformances { get; set; }

    public bool PerVisitor { get; set; }

    public virtual Function FunctionNavigation { get; set; } = null!;
}
