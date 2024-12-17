using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class PrognosisDay
{
    public int? PrognosisId { get; set; }

    public byte Weekday { get; set; }

    public int VisitorEstimate { get; set; }

    public virtual List<Factor> Factors { get; set; } = new List<Factor>();

    public virtual Week Prognosis { get; set; } = null!;

    public virtual List<PrognosisDepartment> PrognosisDepartments { get; set; } = new List<PrognosisDepartment>();
}
