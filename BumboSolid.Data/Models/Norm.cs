using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class Norm
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Activity { get; set; } = null!;

    public string Function { get; set; } = null!;

    [Range(0, int.MaxValue, ErrorMessage = "Duration must be a non-negative value.")]
    public int Duration { get; set; }

    [Range(0, byte.MaxValue, ErrorMessage = "Average Daily Performances must be a non-negative value.")]
    public byte AvgDailyPerformances { get; set; }

    public bool PerVisitor { get; set; }

    public virtual Function? FunctionNavigation { get; set; } = null!;
}