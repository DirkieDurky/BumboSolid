using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Function
{
    public string FunctionName { get; set; } = null!;

    public virtual ICollection<Norm> Norms { get; set; } = new List<Norm>();

    public virtual ICollection<PrognosisFunction> PrognosisFunctions { get; set; } = new List<PrognosisFunction>();
}
