using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class Norm
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [RegularExpression("^[a-zA-Z0-9]*$")]
    public string Activity { get; set; } = null!;

    public string Function { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Duration { get; set; }

    [Range(1, byte.MaxValue)]
    public byte AvgDailyPerformances { get; set; }

    public bool PerVisitor { get; set; }

    public virtual Function? FunctionNavigation { get; set; } = null!;
}