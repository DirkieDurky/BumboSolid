using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class PrognosisDay
{
    public int PrognosisId { get; set; }

    public byte Weekday { get; set; }

    public int VisitorEstimate { get; set; }

    public virtual ICollection<Factor> Factors { get; set; } = new List<Factor>();

    public virtual Prognosis Prognosis { get; set; } = null!;

    public virtual ICollection<PrognosisFunction> PrognosisFunctions { get; set; } = new List<PrognosisFunction>();
}
