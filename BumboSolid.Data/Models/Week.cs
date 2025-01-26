namespace BumboSolid.Data.Models;

public partial class Week
{
    public int Id { get; set; }

    public short Year { get; set; }

    public byte WeekNumber { get; set; }

    public byte HasSchedule { get; set; }

    public virtual List<PrognosisDay> PrognosisDays { get; set; } = [];

    public virtual List<Absence> Absences { get; set; } = [];

    public virtual List<Shift> Shifts { get; set; } = [];

    public virtual List<ClockedHours> ClockedHours { get; set; } = [];

}
