using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class PrognosisFunction
{
    public int PrognosisId { get; set; }

    public string Function { get; set; } = null!;

    public byte Weekday { get; set; }

    public short WorkHours { get; set; }

    public virtual Function FunctionNavigation { get; set; } = null!;

    public virtual PrognosisDay PrognosisDay { get; set; } = null!;
}
