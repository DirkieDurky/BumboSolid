using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class Norm
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Activity can only contain alphanumeric characters.")]
    public string Activity { get; set; } = null!;

    public string Function { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Duration must be a value between 1 and 2147483647.")]
    public int Duration { get; set; }

    [Range(1, byte.MaxValue, ErrorMessage = "Average Daily Performances must be a value between 1 and 255.")]
    public byte AvgDailyPerformances { get; set; }

    public bool PerVisitor { get; set; }

    public virtual Function? FunctionNavigation { get; set; } = null!;
}