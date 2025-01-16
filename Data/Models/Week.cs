namespace BumboSolid.Data.Models;

public partial class Week
{
    public int Id { get; set; }

    public short Year { get; set; }

    public byte WeekNumber { get; set; }

    public byte HasSchedule { get; set; }

    public virtual List<PrognosisDay> PrognosisDays { get; set; } = new List<PrognosisDay>();

    public virtual List<Shift> Shifts { get; set; } = new List<Shift>();

	  public virtual List<Absence> Absences { get; set; } = new List<Absence>();

    public virtual List<ClockedHours> ClockedHours { get; set; } = new List<ClockedHours>();
}
