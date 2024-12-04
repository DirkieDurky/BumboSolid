using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Week
{
    public int Id { get; set; }

    public short Year { get; set; }

    public byte WeekNumber { get; set; }

    public byte HasSchedule { get; set; }

    public virtual List<PrognosisDay> PrognosisDays { get; set; } = new List<PrognosisDay>();

    public virtual List<Shift> Shifts { get; set; } = new List<Shift>();
}
