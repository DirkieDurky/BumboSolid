namespace BumboSolid.Data.Models;

public partial class Prognosis
{
    public int Id { get; set; }

    public short Year { get; set; }

    public byte Week { get; set; }

    public virtual ICollection<PrognosisDay> PrognosisDays { get; set; } = new List<PrognosisDay>();
}
