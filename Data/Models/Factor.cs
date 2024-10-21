using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Factor
{
    public int PrognosisId { get; set; }

    public string Type { get; set; } = null!;

    public byte Weekday { get; set; }

    public byte? WeatherId { get; set; }

    public short Impact { get; set; }

    public string? Description { get; set; }

    public virtual PrognosisDay PrognosisDay { get; set; } = null!;

    public virtual FactorType TypeNavigation { get; set; } = null!;

    public virtual Weather? Weather { get; set; }
}
