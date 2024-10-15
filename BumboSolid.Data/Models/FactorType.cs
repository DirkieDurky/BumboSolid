using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class FactorType
{
    public string Type { get; set; } = null!;

    public virtual ICollection<Factor> Factors { get; set; } = new List<Factor>();
}
