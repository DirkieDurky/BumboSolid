using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class Norm
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [RegularExpression("^[a-zA-Z0-9\\s]*$", ErrorMessage = "Alleen alfanumerieke tekens en spaties zijn toegestaan.")]
    public string Activity { get; set; } = null!;

    public string Function { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Duur moet een waarde tussen 1 en 2147483647 hebben.")]
    public int Duration { get; set; }

    [Range(1, byte.MaxValue, ErrorMessage = "Gemiddeld dagelijks aantal uitvoeringen moet een waarde hebben tussen 1 en 255.")]
    public byte AvgDailyPerformances { get; set; }

    public bool PerVisitor { get; set; }

    public virtual Function? FunctionNavigation { get; set; } = null!;
}